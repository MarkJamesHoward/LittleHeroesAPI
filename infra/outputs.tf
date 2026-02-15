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

output "azure_client_id" {
  description = "Service principal client ID — set as AZURE_CLIENT_ID GitHub secret"
  value       = azuread_application.github_deploy.client_id
}

output "azure_tenant_id" {
  description = "Azure AD tenant ID — set as AZURE_TENANT_ID GitHub secret"
  value       = data.azuread_client_config.current.tenant_id
}

output "azure_subscription_id" {
  description = "Azure subscription ID — set as AZURE_SUBSCRIPTION_ID GitHub secret"
  value       = data.azurerm_subscription.current.subscription_id
}
