output "eks_cluster_endpoint" {
  description = "Endpoint for EKS control plane"
  value       = module.eks.cluster_endpoint
}

output "eks_cluster_name" {
  description = "Kubernetes Cluster Name"
  value       = module.eks.cluster_name
}

output "database_endpoint" {
  description = "Connection endpoint for the RDS Database"
  value       = aws_db_instance.main_db.endpoint
}
