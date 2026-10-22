variable "github_org"   { type = string }
variable "github_repo"  { type = string }
variable "environments" { type = list(string) default = ["dev", "staging", "production"] }

# GitHub OIDC Identity Provider (creates once per AWS Account)
data "tls_certificate" "github" {
  url = "https://token.actions.githubusercontent.com"
}

resource "aws_iam_openid_connect_provider" "github" {
  url             = "https://token.actions.githubusercontent.com"
  client_id_list  = ["sts.amazonaws.com"]
  thumbprint_list = [data.tls_certificate.github.certificates[0].sha1_fingerprint]
}

# Role for CD / Deployment per environment
resource "aws_iam_role" "github_actions_cd" {
  for_each = toset(var.environments)
  name     = "github-actions-cd-${each.value}"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          Federated = aws_iam_openid_connect_provider.github.arn
        }
        Action = "sts:AssumeRoleWithWebIdentity"
        Condition = {
          StringEquals = {
            "token.actions.githubusercontent.com:aud" = "sts.amazonaws.com"
          }
          StringLike = {
            "token.actions.githubusercontent.com:sub" = "repo:${var.github_org}/${var.github_repo}:*"
          }
        }
      }
    ]
  })
}

# Policy allowing ECR Push and EKS Auth
resource "aws_iam_policy" "cd_permissions" {
  name        = "github-actions-cd-permissions"
  description = "Permissions for GitHub Actions to push images and manage EKS apps"

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "ecr:GetAuthorizationToken",
          "ecr:BatchCheckLayerAvailability",
          "ecr:GetDownloadUrlForLayer",
          "ecr:BatchGetImage",
          "ecr:PutImage",
          "ecr:InitiateLayerUpload",
          "ecr:UploadLayerPart",
          "ecr:CompleteLayerUpload"
        ]
        Resource = "*"
      },
      {
        Effect = "Allow"
        Action = [
          "eks:DescribeCluster"
        ]
        Resource = "*"
      }
    ]
  })
}

resource "aws_iam_role_policy_attachment" "attach_cd" {
  for_each   = aws_iam_role.github_actions_cd
  role       = each.value.name
  policy_arn = aws_iam_policy.cd_permissions.arn
}

output "deploy_role_arns" {
  value = { for k, v in aws_iam_role.github_actions_cd : k => v.arn }
}
