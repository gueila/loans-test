output "alb_dns_name" {
  description = "DNS name of the ALB (application endpoint)"
  value       = aws_lb.fintech.dns_name
}

output "ecr_repository_url" {
  description = "ECR repository URL for the application image"
  value       = aws_ecr_repository.fintech.repository_url
}

output "rds_endpoint" {
  description = "RDS database endpoint"
  value       = aws_db_instance.fintech.endpoint
}

output "rds_database_name" {
  description = "RDS database name"
  value       = aws_db_instance.fintech.db_name
}

output "vpc_id" {
  description = "VPC ID"
  value       = aws_vpc.fintech.id
}
