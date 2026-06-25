resource "aws_secretsmanager_secret" "db_connection" {
  name        = "${var.project_name}/${var.environment}/db-connection"
  description = "Database connection string for FinTech API"
  tags = { Name = "${var.project_name}-${var.environment}-db-secret" }
}

resource "aws_secretsmanager_secret_version" "db_connection" {
  secret_id = aws_secretsmanager_secret.db_connection.id
  secret_string = "Host=${aws_db_instance.fintech.address};Port=5432;Database=${var.db_name};Username=${var.db_username};Password=${var.db_password}"
}

resource "aws_secretsmanager_secret" "jwt" {
  name        = "${var.project_name}/${var.environment}/jwt"
  description = "JWT configuration for FinTech API"
  tags = { Name = "${var.project_name}-${var.environment}-jwt-secret" }
}

resource "aws_secretsmanager_secret_version" "jwt" {
  secret_id = aws_secretsmanager_secret.jwt.id
  secret_string = jsonencode({
    key      = var.jwt_key
    issuer   = var.jwt_issuer
    audience = var.jwt_audience
  })
}
