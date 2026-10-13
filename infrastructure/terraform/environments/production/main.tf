# ─────────────────────────────────────────────────────────────────────────────
# Production Environment — Terraform Entry Point
# This file calls shared modules with production-grade settings
# ─────────────────────────────────────────────────────────────────────────────

locals {
  environment = "production"
  region      = "us-east-1"
}

module "vpc" {
  source = "../../modules/vpc"

  environment         = local.environment
  vpc_cidr            = "10.2.0.0/16"
  availability_zones  = ["us-east-1a", "us-east-1b", "us-east-1c"]
  private_subnets     = ["10.2.1.0/24", "10.2.2.0/24", "10.2.3.0/24"]
  public_subnets      = ["10.2.101.0/24", "10.2.102.0/24", "10.2.103.0/24"]
  enable_nat_gateway  = true
  single_nat_gateway  = false  # HA: one NAT GW per AZ in production
  enable_vpn_gateway  = false
  enable_flow_logs    = true
}

module "eks" {
  source = "../../modules/eks"

  environment        = local.environment
  cluster_name       = "smart-trade-prod"
  cluster_version    = "1.30"
  vpc_id             = module.vpc.vpc_id
  private_subnet_ids = module.vpc.private_subnet_ids
  public_subnet_ids  = module.vpc.public_subnet_ids

  # Production node groups — multiple sizes for different workloads
  node_groups = {
    system = {
      instance_types = ["m5.large"]
      capacity_type  = "ON_DEMAND"
      desired_size   = 3
      min_size       = 3
      max_size       = 6
      labels         = { role = "system" }
      taints         = []
    }
    application = {
      instance_types = ["m5.xlarge"]
      capacity_type  = "ON_DEMAND"
      desired_size   = 4
      min_size       = 3
      max_size       = 20
      labels         = { role = "application" }
      taints         = []
    }
    spot-batch = {
      instance_types = ["m5.large", "m5a.large", "m4.large"]
      capacity_type  = "SPOT"
      desired_size   = 2
      min_size       = 0
      max_size       = 10
      labels         = { role = "batch" }
      taints = [{
        key    = "workload"
        value  = "batch"
        effect = "NO_SCHEDULE"
      }]
    }
  }

  # Production add-ons
  enable_cluster_autoscaler   = true
  enable_karpenter            = false   # Set to true if using Karpenter instead of CA
  enable_aws_load_balancer_controller = true
  enable_external_dns         = true
  enable_cert_manager         = true
  enable_metrics_server       = true
  enable_aws_ebs_csi_driver   = true

  # Security
  cluster_endpoint_private_access = true
  cluster_endpoint_public_access  = false  # VPN-only in production
  enable_irsa                     = true   # IAM Roles for Service Accounts
}

module "rds" {
  source = "../../modules/rds"

  environment              = local.environment
  identifier               = "smart-trade-prod"
  engine                   = "postgres"
  engine_version           = "16.3"
  instance_class           = "db.r6g.xlarge"
  allocated_storage        = 100
  max_allocated_storage    = 500   # Auto-scaling storage
  multi_az                 = true  # HA in production
  deletion_protection      = true
  backup_retention_period  = 30
  backup_window            = "03:00-04:00"
  maintenance_window       = "sun:04:00-sun:05:00"
  skip_final_snapshot      = false
  final_snapshot_identifier = "smart-trade-prod-final"
  performance_insights_enabled = true
  monitoring_interval      = 60   # Enhanced monitoring every minute
  vpc_id                   = module.vpc.vpc_id
  subnet_ids               = module.vpc.private_subnet_ids
  allowed_security_groups  = [module.eks.node_security_group_id]

  # Production: password from Secrets Manager, not tfvars
  manage_master_user_password = true
}

module "elasticache" {
  source = "../../modules/elasticache"

  environment        = local.environment
  cluster_id         = "smart-trade-prod"
  node_type          = "cache.r6g.large"
  num_cache_nodes    = 3
  automatic_failover = true
  multi_az_enabled   = true
  vpc_id             = module.vpc.vpc_id
  subnet_ids         = module.vpc.private_subnet_ids
  allowed_security_groups = [module.eks.node_security_group_id]
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

  # Lifecycle: keep last 10 tagged images, remove untagged after 1 day
  image_tag_mutability = "IMMUTABLE"
  scan_on_push         = true
}

module "iam_github_oidc" {
  source = "../../modules/iam-github-oidc"

  github_org  = "your-org"
  github_repo = "smart-trade-logistics-platform"
  environments = ["dev", "staging", "production"]
}
