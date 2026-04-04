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

  identity {
    type = "SystemAssigned"
  }

  app_settings = {
    "ASPNETCORE_ENVIRONMENT"               = "Production"
    "ConnectionStrings__DefaultConnection" = "Server=tcp:${azurerm_mssql_server.main.fully_qualified_domain_name},1433;Initial Catalog=${var.sql_database_name};Encrypt=True;TrustServerCertificate=False;Authentication=Active Directory Default;"
  }
}

# -------------------------------------------------------
# Azure SQL Server
# -------------------------------------------------------
resource "azurerm_mssql_server" "main" {
  name                         = var.sql_server_name
  location                     = azurerm_resource_group.main.location
  resource_group_name          = azurerm_resource_group.main.name
  version                      = "12.0"
  administrator_login          = var.sql_admin_username
  administrator_login_password = var.sql_admin_password

  azuread_administrator {
    login_username = "AzureAD Admin"
    object_id      = data.azuread_client_config.current.object_id
  }
}

# -------------------------------------------------------
# Azure SQL Database (Free serverless tier)
# -------------------------------------------------------
resource "azurerm_mssql_database" "main" {
  name      = var.sql_database_name
  server_id = azurerm_mssql_server.main.id
  sku_name  = "GP_S_Gen5_2"

  auto_pause_delay_in_minutes = 60
  min_capacity                = 0.5
  max_size_gb                 = 32
}

# -------------------------------------------------------
# Firewall Rule - Allow Azure Services
# -------------------------------------------------------
resource "azurerm_mssql_firewall_rule" "allow_azure" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.main.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
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
