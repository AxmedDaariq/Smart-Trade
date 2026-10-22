# ─────────────────────────────────────────────────────────────────────────────
# Reusable EKS Module
# ─────────────────────────────────────────────────────────────────────────────

variable "environment"        { type = string }
variable "cluster_name"       { type = string }
variable "cluster_version"    { type = string  default = "1.30" }
variable "vpc_id"             { type = string }
variable "private_subnet_ids" { type = list(string) }
variable "public_subnet_ids"  { type = list(string) }
variable "node_groups"        { type = any }
variable "enable_cluster_autoscaler"              { type = bool  default = true }
variable "enable_karpenter"                       { type = bool  default = false }
variable "enable_aws_load_balancer_controller"    { type = bool  default = true }
variable "enable_external_dns"                    { type = bool  default = true }
variable "enable_cert_manager"                    { type = bool  default = true }
variable "enable_metrics_server"                  { type = bool  default = true }
variable "enable_aws_ebs_csi_driver"              { type = bool  default = true }
variable "cluster_endpoint_private_access"        { type = bool  default = true }
variable "cluster_endpoint_public_access"         { type = bool  default = false }
variable "enable_irsa"                            { type = bool  default = true }

module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = "~> 20.0"

  cluster_name    = var.cluster_name
  cluster_version = var.cluster_version

  vpc_id                   = var.vpc_id
  subnet_ids               = var.private_subnet_ids
  control_plane_subnet_ids = var.public_subnet_ids

  cluster_endpoint_private_access = var.cluster_endpoint_private_access
  cluster_endpoint_public_access  = var.cluster_endpoint_public_access

  # Enable IRSA (IAM Roles for Service Accounts)
  enable_irsa = var.enable_irsa

  # Encryption config — all secrets at rest
  cluster_encryption_config = {
    provider_key_arn = aws_kms_key.eks.arn
    resources        = ["secrets"]
  }

  # EKS Managed Node Groups
  eks_managed_node_groups = var.node_groups

  # Cluster add-ons
  cluster_addons = {
    coredns = {
      addon_version = "v1.11.1-eksbuild.4"
      resolve_conflicts_on_update = "OVERWRITE"
    }
    kube-proxy = {
      addon_version = "v1.30.0-eksbuild.3"
    }
    vpc-cni = {
      addon_version = "v1.18.1-eksbuild.3"
      resolve_conflicts_on_update = "OVERWRITE"
    }
    aws-ebs-csi-driver = {
      addon_version = "v1.31.0-eksbuild.1"
      service_account_role_arn = var.enable_aws_ebs_csi_driver ? aws_iam_role.ebs_csi[0].arn : null
    }
  }

  # Node security group rules
  node_security_group_additional_rules = {
    ingress_allow_access_from_control_plane = {
      type                          = "ingress"
      protocol                      = "tcp"
      from_port                     = 9443
      to_port                       = 9443
      source_cluster_security_group = true
      description                   = "Allow access from control plane to webhook port"
    }
  }

  tags = {
    Environment = var.environment
  }
}

# KMS key for EKS secrets encryption
resource "aws_kms_key" "eks" {
  description             = "EKS secrets encryption — ${var.cluster_name}"
  deletion_window_in_days = 7
  enable_key_rotation     = true

  tags = {
    Name        = "${var.cluster_name}-eks-kms"
    Environment = var.environment
  }
}

resource "aws_kms_alias" "eks" {
  name          = "alias/${var.cluster_name}-eks"
  target_key_id = aws_kms_key.eks.key_id
}

# EBS CSI Driver IAM role
resource "aws_iam_role" "ebs_csi" {
  count = var.enable_aws_ebs_csi_driver ? 1 : 0
  name  = "${var.cluster_name}-ebs-csi-driver"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect = "Allow"
      Principal = {
        Federated = module.eks.oidc_provider_arn
      }
      Action = "sts:AssumeRoleWithWebIdentity"
      Condition = {
        StringEquals = {
          "${module.eks.oidc_provider}:aud" = "sts.amazonaws.com"
          "${module.eks.oidc_provider}:sub" = "system:serviceaccount:kube-system:ebs-csi-controller-sa"
        }
      }
    }]
  })
}

resource "aws_iam_role_policy_attachment" "ebs_csi" {
  count      = var.enable_aws_ebs_csi_driver ? 1 : 0
  role       = aws_iam_role.ebs_csi[0].name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonEBSCSIDriverPolicy"
}

# ─────────────────────────────────────────────────────────────────────────────
# OUTPUTS
# ─────────────────────────────────────────────────────────────────────────────
output "cluster_id"               { value = module.eks.cluster_id }
output "cluster_endpoint"         { value = module.eks.cluster_endpoint }
output "cluster_name"             { value = module.eks.cluster_name }
output "cluster_certificate_authority_data" { value = module.eks.cluster_certificate_authority_data }
output "oidc_provider_arn"        { value = module.eks.oidc_provider_arn }
output "oidc_provider"            { value = module.eks.oidc_provider }
output "node_security_group_id"   { value = module.eks.node_security_group_id }
output "cluster_security_group_id" { value = module.eks.cluster_security_group_id }
