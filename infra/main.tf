terraform {
  required_version = ">= 1.5.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
    azuread = {
      source  = "hashicorp/azuread"
      version = "~> 3.0"
    }
    github = {
      source  = "integrations/github"
      version = "~> 6.0"
    }
  }
}

provider "azurerm" {
  features {}
}

provider "azuread" {}

provider "github" {
  owner = split("/", var.github_repo)[0]
}

data "azurerm_subscription" "current" {}
data "azuread_client_config" "current" {}

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
    "ASPNETCORE_ENVIRONMENT"               = "Production"
    "ConnectionStrings__DefaultConnection" = "Server=${azurerm_mysql_flexible_server.main.fqdn};Database=${var.mysql_database_name};User=${var.mysql_admin_username};Password=${var.mysql_admin_password};SslMode=Required;"
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

# -------------------------------------------------------
# Azure AD App Registration & Service Principal (for GitHub Actions OIDC)
# -------------------------------------------------------
resource "azuread_application" "github_deploy" {
  display_name = "${var.app_name}-github-deploy"
  owners       = [data.azuread_client_config.current.object_id]
}

resource "azuread_service_principal" "github_deploy" {
  client_id = azuread_application.github_deploy.client_id
  owners    = [data.azuread_client_config.current.object_id]
}

resource "azuread_application_federated_identity_credential" "github_main" {
  application_id = azuread_application.github_deploy.id
  display_name   = "github-actions-main"
  description    = "GitHub Actions deploying from main branch"
  audiences      = ["api://AzureADTokenExchange"]
  issuer         = "https://token.actions.githubusercontent.com"
  subject        = "repo:${var.github_repo}:ref:refs/heads/main"
}

# -------------------------------------------------------
# Role Assignment - Contributor on Resource Group
# -------------------------------------------------------
resource "azurerm_role_assignment" "github_deploy_contributor" {
  scope                = azurerm_resource_group.main.id
  role_definition_name = "Contributor"
  principal_id         = azuread_service_principal.github_deploy.object_id
}

# -------------------------------------------------------
# GitHub Actions Secrets (OIDC credentials)
# -------------------------------------------------------
resource "github_actions_secret" "azure_client_id" {
  repository      = split("/", var.github_repo)[1]
  secret_name     = "AZURE_CLIENT_ID"
  plaintext_value = azuread_application.github_deploy.client_id
}

resource "github_actions_secret" "azure_tenant_id" {
  repository      = split("/", var.github_repo)[1]
  secret_name     = "AZURE_TENANT_ID"
  plaintext_value = data.azuread_client_config.current.tenant_id
}

resource "github_actions_secret" "azure_subscription_id" {
  repository      = split("/", var.github_repo)[1]
  secret_name     = "AZURE_SUBSCRIPTION_ID"
  plaintext_value = data.azurerm_subscription.current.subscription_id
}
