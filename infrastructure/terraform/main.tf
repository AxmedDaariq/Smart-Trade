provider "aws" {
  region = var.aws_region
}

# 1. Abuurista Shabakadda (VPC & Subnets)
module "vpc" {
  source  = "terraform-aws-modules/vpc/aws"
  version = "5.0.0"

  name = "smart-trade-vpc"
  cidr = var.vpc_cidr

  azs             = ["${var.aws_region}a", "${var.aws_region}b"]
  private_subnets = ["10.0.1.0/24", "10.0.2.0/24"]
  public_subnets  = ["10.0.101.0/24", "10.0.102.0/24"]

  enable_nat_gateway = true
  single_nat_gateway = true
}

# 2. Abuurista Kubernetes Cluster (AWS EKS)
module "eks" {
  source  = "terraform-aws-modules/eks/aws"
  version = "19.15.3"

  cluster_name    = var.cluster_name
  cluster_version = "1.27"
  
  vpc_id                   = module.vpc.vpc_id
  subnet_ids               = module.vpc.private_subnets
  control_plane_subnet_ids = module.vpc.public_subnets

  # Qaybta Servers-ka (Node Groups)
  eks_managed_node_groups = {
    general_nodes = {
      desired_size = 2
      min_size     = 1
      max_size     = 4
      
      instance_types = ["t3.medium"]
    }
  }
}

# 3. Abuurista Database-ka (AWS RDS - PostgreSQL tusaale ahaan)
resource "aws_db_instance" "main_db" {
  identifier           = "smart-trade-db"
  allocated_storage    = 20
  engine               = "postgres"
  engine_version       = "15.3"
  instance_class       = "db.t3.micro"
  username             = "postgresadmin"
  password             = var.db_password
  parameter_group_name = "default.postgres15"
  skip_final_snapshot  = true
  
  db_subnet_group_name = module.vpc.database_subnet_group_name
  vpc_security_group_ids = [module.vpc.default_security_group_id]
}
