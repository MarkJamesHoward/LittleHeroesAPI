variable "resource_group_name" {
  description = "Name of the Azure resource group"
  type        = string
  default     = "rg-littleheroes"
}

variable "location" {
  description = "Azure region for all resources"
  type        = string
  default     = "newzealandnorth"
}

variable "app_name" {
  description = "Name of the App Service (must be globally unique)"
  type        = string
  default     = "littleheroesapi"
}

variable "sql_server_name" {
  description = "Name of the Azure SQL Server (must be globally unique)"
  type        = string
  default     = "littleheroes-sql"
}

variable "sql_database_name" {
  description = "Name of the SQL database"
  type        = string
  default     = "littleheroes"
}

variable "sql_admin_username" {
  description = "Administrator username for Azure SQL Server"
  type        = string
  sensitive   = true
}

variable "sql_admin_password" {
  description = "Administrator password for Azure SQL Server"
  type        = string
  sensitive   = true
}

variable "github_repo" {
  description = "GitHub repository in 'owner/name' format for OIDC federation"
  type        = string
  default     = "MarkJamesHoward/LittleHeroesAPI"
}
