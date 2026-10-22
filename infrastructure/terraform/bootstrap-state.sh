# ─────────────────────────────────────────────────────────────────────────────
# Bootstrap script: creates S3 bucket + DynamoDB table for Terraform remote state
# Run ONCE manually before first `terraform init`
# Usage: bash bootstrap-state.sh <environment>
# ─────────────────────────────────────────────────────────────────────────────

#!/usr/bin/env bash
set -euo pipefail

ENVIRONMENT="${1:-dev}"
AWS_REGION="${AWS_REGION:-us-east-1}"
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
BUCKET_NAME="smart-trade-tfstate-${ENVIRONMENT}"
TABLE_NAME="smart-trade-tfstate-lock-${ENVIRONMENT}"

echo "🔧 Bootstrapping Terraform state for environment: ${ENVIRONMENT}"
echo "   Account: ${ACCOUNT_ID}"
echo "   Region:  ${AWS_REGION}"
echo "   Bucket:  ${BUCKET_NAME}"
echo "   Table:   ${TABLE_NAME}"

# Create S3 bucket
if ! aws s3api head-bucket --bucket "${BUCKET_NAME}" 2>/dev/null; then
  aws s3api create-bucket \
    --bucket "${BUCKET_NAME}" \
    --region "${AWS_REGION}" \
    $([[ "${AWS_REGION}" != "us-east-1" ]] && echo "--create-bucket-configuration LocationConstraint=${AWS_REGION}" || echo "")

  # Enable versioning
  aws s3api put-bucket-versioning \
    --bucket "${BUCKET_NAME}" \
    --versioning-configuration Status=Enabled

  # Enable server-side encryption (AES-256)
  aws s3api put-bucket-encryption \
    --bucket "${BUCKET_NAME}" \
    --server-side-encryption-configuration '{
      "Rules": [{
        "ApplyServerSideEncryptionByDefault": {
          "SSEAlgorithm": "aws:kms"
        },
        "BucketKeyEnabled": true
      }]
    }'

  # Block all public access
  aws s3api put-public-access-block \
    --bucket "${BUCKET_NAME}" \
    --public-access-block-configuration \
      "BlockPublicAcls=true,IgnorePublicAcls=true,BlockPublicPolicy=true,RestrictPublicBuckets=true"

  # Enable object lock (prevent accidental deletion of state files)
  aws s3api put-bucket-policy \
    --bucket "${BUCKET_NAME}" \
    --policy "{
      \"Version\": \"2012-10-17\",
      \"Statement\": [{
        \"Effect\": \"Deny\",
        \"Principal\": \"*\",
        \"Action\": \"s3:DeleteObject\",
        \"Resource\": \"arn:aws:s3:::${BUCKET_NAME}/*\",
        \"Condition\": {
          \"StringNotEquals\": {
            \"aws:PrincipalArn\": \"arn:aws:iam::${ACCOUNT_ID}:root\"
          }
        }
      }]
    }"

  echo "✅ S3 bucket created: ${BUCKET_NAME}"
else
  echo "ℹ️  S3 bucket already exists: ${BUCKET_NAME}"
fi

# Create DynamoDB table for state locking
if ! aws dynamodb describe-table --table-name "${TABLE_NAME}" 2>/dev/null; then
  aws dynamodb create-table \
    --table-name "${TABLE_NAME}" \
    --attribute-definitions AttributeName=LockID,AttributeType=S \
    --key-schema AttributeName=LockID,KeyType=HASH \
    --billing-mode PAY_PER_REQUEST \
    --region "${AWS_REGION}"

  # Enable point-in-time recovery
  aws dynamodb update-continuous-backups \
    --table-name "${TABLE_NAME}" \
    --point-in-time-recovery-specification PointInTimeRecoveryEnabled=true

  echo "✅ DynamoDB table created: ${TABLE_NAME}"
else
  echo "ℹ️  DynamoDB table already exists: ${TABLE_NAME}"
fi

echo ""
echo "✅ Bootstrap complete. You can now run:"
echo "   terraform init \\"
echo "     -backend-config=\"bucket=${BUCKET_NAME}\" \\"
echo "     -backend-config=\"key=${ENVIRONMENT}/terraform.tfstate\" \\"
echo "     -backend-config=\"region=${AWS_REGION}\" \\"
echo "     -backend-config=\"dynamodb_table=${TABLE_NAME}\" \\"
echo "     -backend-config=\"encrypt=true\""
