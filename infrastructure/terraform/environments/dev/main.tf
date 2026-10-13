# ─────────────────────────────────────────────────────────────────────────────
# Dev Environment — Terraform Entry Point
# Smaller, cheaper resources suitable for development
# ─────────────────────────────────────────────────────────────────────────────

locals {
  environment = "dev"
  region      = "us-east-1"
}

module "vpc" {
  source = "../../modules/vpc"

  environment        = local.environment
  vpc_cidr           = "10.0.0.0/16"
  availability_zones = ["us-east-1a", "us-east-1b"]
  private_subnets    = ["10.0.1.0/24", "10.0.2.0/24"]
  public_subnets     = ["10.0.101.0/24", "10.0.102.0/24"]
  enable_nat_gateway = true
  single_nat_gateway = true  # Cost-saving in dev
  enable_flow_logs   = false
}

module "eks" {
  source = "../../modules/eks"

  environment        = local.environment
  cluster_name       = "smart-trade-dev"
  cluster_version    = "1.30"
  vpc_id             = module.vpc.vpc_id
  private_subnet_ids = module.vpc.private_subnet_ids
  public_subnet_ids  = module.vpc.public_subnet_ids

  node_groups = {
    general = {
      instance_types = ["t3.medium"]
      capacity_type  = "SPOT"
      desired_size   = 2
      min_size       = 1
      max_size       = 6
      labels         = { role = "general" }
      taints         = []
    }
  }

  enable_cluster_autoscaler              = true
  enable_aws_load_balancer_controller    = true
  enable_external_dns                    = true
  enable_cert_manager                    = true
  enable_metrics_server                  = true
  enable_aws_ebs_csi_driver              = true
  cluster_endpoint_private_access        = true
  cluster_endpoint_public_access         = true  # Dev only — allow dev machine access
  enable_irsa                            = true
}

module "rds" {
  source = "../../modules/rds"

  environment             = local.environment
  identifier              = "smart-trade-dev"
  engine                  = "postgres"
  engine_version          = "16.3"
  instance_class          = "db.t3.medium"
  allocated_storage       = 20
  max_allocated_storage   = 50
  multi_az                = false
  deletion_protection     = false
  backup_retention_period = 3
  skip_final_snapshot     = true
  vpc_id                  = module.vpc.vpc_id
  subnet_ids              = module.vpc.private_subnet_ids
  allowed_security_groups = [module.eks.node_security_group_id]

  manage_master_user_password = true
}

module "ecr" {
  source = "../../modules/ecr"

  environment = local.environment
  services = [
    "identity", "customs", "shipping", "payment", "notification",
    "fleet", "gps", "gps-telematics", "audit", "declaration",
    "analytics", "corridor", "dryport", "ectn", "anpr-gate",
    "ais-vessel", "integration"
  ]

  image_tag_mutability = "MUTABLE"
  scan_on_push         = true
}

module "iam_github_oidc" {
  source = "../../modules/iam-github-oidc"

  github_org  = "your-org"
  github_repo = "smart-trade-logistics-platform"
  environments = ["dev"]
}
