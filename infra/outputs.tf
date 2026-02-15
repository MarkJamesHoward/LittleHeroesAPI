output "app_service_url" {
  description = "URL of the deployed App Service"
  value       = "https://${azurerm_linux_web_app.main.default_hostname}"
}

output "mysql_server_fqdn" {
  description = "FQDN of the MySQL Flexible Server"
  value       = azurerm_mysql_flexible_server.main.fqdn
}

output "mysql_connection_string" {
  description = "MySQL connection string (contains credentials)"
  value       = "Server=${azurerm_mysql_flexible_server.main.fqdn};Database=${var.mysql_database_name};User=${var.mysql_admin_username};Password=${var.mysql_admin_password};SslMode=Required;"
  sensitive   = true
}
