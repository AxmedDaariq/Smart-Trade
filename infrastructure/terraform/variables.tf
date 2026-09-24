variable "aws_region" {
  description = "Region-ka AWS oo laga samaynayo kheyraadka (AWS Region)"
  type        = string
  default     = "us-east-1"
}

variable "cluster_name" {
  description = "Magaca Kubernetes Cluster-ka (EKS Cluster Name)"
  type        = string
  default     = "smart-trade-cluster"
}

variable "db_password" {
  description = "Furaha sirta ah ee Database-ka (Database Password)"
  type        = string
  sensitive   = true
}

variable "vpc_cidr" {
  description = "Balka isku-xirka (VPC CIDR block)"
  type        = string
  default     = "10.0.0.0/16"
}
