variable "environment"                 { type = string }
variable "identifier"                  { type = string }
variable "engine"                      { type = string default = "postgres" }
variable "engine_version"              { type = string default = "16.3" }
variable "instance_class"              { type = string }
variable "allocated_storage"           { type = number default = 20 }
variable "max_allocated_storage"       { type = number default = 100 }
variable "multi_az"                    { type = bool default = false }
variable "deletion_protection"         { type = bool default = false }
variable "backup_retention_period"     { type = number default = 7 }
variable "backup_window"               { type = string default = "03:00-04:00" }
variable "maintenance_window"          { type = string default = "sun:04:00-sun:05:00" }
variable "skip_final_snapshot"         { type = bool default = true }
variable "final_snapshot_identifier"   { type = string default = null }
variable "performance_insights_enabled" { type = bool default = false }
variable "monitoring_interval"         { type = number default = 0 }
variable "vpc_id"                      { type = string }
variable "subnet_ids"                  { type = list(string) }
variable "allowed_security_groups"     { type = list(string) default = [] }
variable "manage_master_user_password" { type = bool default = true }

resource "aws_db_subnet_group" "rds" {
  name       = "${var.identifier}-subnet-group"
  subnet_ids = var.subnet_ids

  tags = {
    Name        = "${var.identifier}-subnet-group"
    Environment = var.environment
  }
}

resource "aws_security_group" "rds" {
  name        = "${var.identifier}-rds-sg"
  description = "Control traffic to RDS Postgres"
  vpc_id      = var.vpc_id

  ingress {
    description     = "PostgreSQL from permitted security groups (EKS Nodes)"
    from_port       = 5432
    to_port         = 5432
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
    Name        = "${var.identifier}-rds-sg"
    Environment = var.environment
  }
}

resource "aws_db_instance" "rds" {
  identifier                  = var.identifier
  engine                      = var.engine
  engine_version              = var.engine_version
  instance_class              = var.instance_class
  allocated_storage           = var.allocated_storage
  max_allocated_storage       = var.max_allocated_storage
  storage_type                = "gp3"
  multi_az                    = var.multi_az
  deletion_protection         = var.deletion_protection

  db_name                     = "smarttrade_${var.environment}"
  username                    = "dbadmin"
  manage_master_user_password = var.manage_master_user_password

  db_subnet_group_name        = aws_db_subnet_group.rds.name
  vpc_security_group_ids      = [aws_security_group.rds.id]

  backup_retention_period     = var.backup_retention_period
  backup_window               = var.backup_window
  maintenance_window          = var.maintenance_window
  skip_final_snapshot         = var.skip_final_snapshot
  final_snapshot_identifier   = var.final_snapshot_identifier

  performance_insights_enabled = var.performance_insights_enabled
  monitoring_interval          = var.monitoring_interval

  auto_minor_version_upgrade  = true
  publicly_accessible         = false
  copy_tags_to_snapshot       = true

  tags = {
    Name        = var.identifier
    Environment = var.environment
  }
}

output "db_instance_id"       { value = aws_db_instance.rds.id }
output "db_instance_endpoint" { value = aws_db_instance.rds.endpoint }
output "db_instance_address"  { value = aws_db_instance.rds.address }
output "db_instance_port"     { value = aws_db_instance.rds.port }
output "db_security_group_id" { value = aws_security_group.rds.id }
