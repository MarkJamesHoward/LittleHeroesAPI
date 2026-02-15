terraform {
  required_version = ">= 1.5.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# -------------------------------------------------------
# Resource Group
# -------------------------------------------------------
resource "azurerm_resource_group" "main" {
  name     = var.resource_group_name
  location = var.location
}

# -------------------------------------------------------
# App Service Plan (Free F1, Linux)
# -------------------------------------------------------
resource "azurerm_service_plan" "main" {
  name                = "${var.app_name}-plan"
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  os_type             = "Linux"
  sku_name            = "F1"
}

# -------------------------------------------------------
# App Service (Linux, .NET 10)
# -------------------------------------------------------
resource "azurerm_linux_web_app" "main" {
  name                = var.app_name
  location            = azurerm_resource_group.main.location
  resource_group_name = azurerm_resource_group.main.name
  service_plan_id     = azurerm_service_plan.main.id

  site_config {
    application_stack {
      dotnet_version = "10.0"
    }
    always_on = false # F1 tier does not support Always On
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = "Production"
  }

  connection_string {
    name  = "DefaultConnection"
    type  = "MySql"
    value = "Server=${azurerm_mysql_flexible_server.main.fqdn};Database=${var.mysql_database_name};User=${var.mysql_admin_username};Password=${var.mysql_admin_password};SslMode=Required;"
  }
}

# -------------------------------------------------------
# MySQL Flexible Server
# -------------------------------------------------------
resource "azurerm_mysql_flexible_server" "main" {
  name                   = var.mysql_server_name
  location               = azurerm_resource_group.main.location
  resource_group_name    = azurerm_resource_group.main.name
  administrator_login    = var.mysql_admin_username
  administrator_password = var.mysql_admin_password
  sku_name               = var.mysql_sku_name
  version                = "8.0.21"

  storage {
    size_gb = 20
  }

  backup_retention_days = 7
}

# -------------------------------------------------------
# MySQL Database
# -------------------------------------------------------
resource "azurerm_mysql_flexible_database" "main" {
  name                = var.mysql_database_name
  resource_group_name = azurerm_resource_group.main.name
  server_name         = azurerm_mysql_flexible_server.main.name
  charset             = "utf8mb4"
  collation           = "utf8mb4_unicode_ci"
}

# -------------------------------------------------------
# Firewall Rule - Allow Azure Services
# -------------------------------------------------------
resource "azurerm_mysql_flexible_server_firewall_rule" "allow_azure" {
  name                = "AllowAzureServices"
  server_name         = azurerm_mysql_flexible_server.main.name
  resource_group_name = azurerm_resource_group.main.name
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}
