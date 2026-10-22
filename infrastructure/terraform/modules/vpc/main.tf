variable "environment"        { type = string }
variable "vpc_cidr"           { type = string }
variable "availability_zones" { type = list(string) }
variable "private_subnets"    { type = list(string) }
variable "public_subnets"     { type = list(string) }
variable "enable_nat_gateway" { type = bool default = true }
variable "single_nat_gateway" { type = bool default = false }
variable "enable_vpn_gateway" { type = bool default = false }
variable "enable_flow_logs"   { type = bool default = true }

module "vpc" {
  source  = "terraform-aws-modules/vpc/aws"
  version = "~> 5.0"

  name = "smart-trade-vpc-${var.environment}"
  cidr = var.vpc_cidr

  azs             = var.availability_zones
  private_subnets = var.private_subnets
  public_subnets  = var.public_subnets

  enable_nat_gateway = var.enable_nat_gateway
  single_nat_gateway = var.single_nat_gateway
  enable_vpn_gateway = var.enable_vpn_gateway

  enable_dns_hostnames = true
  enable_dns_support   = true

  # Tagging for EKS Load Balancer Controller discovery
  public_subnet_tags = {
    "kubernetes.io/role/elb"                               = "1"
    "kubernetes.io/cluster/smart-trade-${var.environment}" = "shared"
  }

  private_subnet_tags = {
    "kubernetes.io/role/internal-elb"                      = "1"
    "kubernetes.io/cluster/smart-trade-${var.environment}" = "shared"
  }

  tags = {
    Environment = var.environment
  }
}

output "vpc_id"             { value = module.vpc.vpc_id }
output "private_subnet_ids" { value = module.vpc.private_subnets }
output "public_subnet_ids"  { value = module.vpc.public_subnets }
