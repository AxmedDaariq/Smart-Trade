variable "environment"             { type = string }
variable "cluster_id"              { type = string }
variable "node_type"               { type = string default = "cache.t4g.medium" }
variable "num_cache_nodes"         { type = number default = 1 }
variable "automatic_failover"      { type = bool default = false }
variable "multi_az_enabled"        { type = bool default = false }
variable "vpc_id"                  { type = string }
variable "subnet_ids"              { type = list(string) }
variable "allowed_security_groups" { type = list(string) default = [] }

resource "aws_elasticache_subnet_group" "redis" {
  name       = "${var.cluster_id}-redis-subnet-group"
  subnet_ids = var.subnet_ids

  tags = {
    Environment = var.environment
  }
}

resource "aws_security_group" "redis" {
  name        = "${var.cluster_id}-redis-sg"
  description = "Redis ingress from EKS nodes"
  vpc_id      = var.vpc_id

  ingress {
    from_port       = 6379
    to_port         = 6379
    protocol        = "tcp"
    security_groups = var.allowed_security_groups
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Environment = var.environment
  }
}

resource "aws_elasticache_replication_group" "redis" {
  replication_group_id       = "${var.cluster_id}-redis"
  description                = "Redis cluster for ${var.environment}"
  node_type                  = var.node_type
  num_cache_clusters         = var.num_cache_nodes
  parameter_group_name       = "default.redis7"
  port                       = 6379
  automatic_failover_enabled = var.automatic_failover
  multi_az_enabled           = var.multi_az_enabled

  subnet_group_name          = aws_elasticache_subnet_group.redis.name
  security_group_ids         = [aws_security_group.redis.id]

  at_rest_encryption_enabled = true
  transit_encryption_enabled = true

  tags = {
    Environment = var.environment
  }
}

output "redis_primary_endpoint" {
  value = aws_elasticache_replication_group.redis.primary_endpoint_address
}
output "redis_port" {
  value = aws_elasticache_replication_group.redis.port
}
