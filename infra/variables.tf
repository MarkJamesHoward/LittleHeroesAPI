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

variable "mysql_admin_username" {
  description = "Administrator username for MySQL Flexible Server"
  type        = string
  sensitive   = true
}

variable "mysql_admin_password" {
  description = "Administrator password for MySQL Flexible Server"
  type        = string
  sensitive   = true
}

variable "mysql_server_name" {
  description = "Name of the MySQL Flexible Server (must be globally unique)"
  type        = string
  default     = "littleheroes-mysql"
}

variable "mysql_database_name" {
  description = "Name of the MySQL database"
  type        = string
  default     = "littleheroes"
}

variable "mysql_sku_name" {
  description = "SKU for MySQL Flexible Server"
  type        = string
  default     = "B_Standard_B1ms"
}

variable "github_repo" {
  description = "GitHub repository in 'owner/name' format for OIDC federation"
  type        = string
  default     = "MarkJamesHoward/LittleHeroesAPI"
}
