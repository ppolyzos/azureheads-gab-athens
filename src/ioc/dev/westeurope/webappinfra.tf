# We strongly recommend using the required_providers block to set the
# Azure Provider source and version being used
terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.46.0"
    }
  }
}

# Configure the Microsoft Azure Provider
provider "azurerm" {
  features {}
}

locals {
  name        = "evt-mgmt-app"
  region      = "West Europe"
  owner       = "ppol"
  environment = "dev"
}

locals {
  # Common tags to be assigned to all resources
  common_tags = {
    "CreatedBy"   = local.owner
    "Environment" = local.environment
  }
}

resource "random_id" "server" {
  keepers = {
    # Generate a new id each time we switch to a new Azure Resource Group
    rg_id = local.name
  }

  byte_length = 8
}

# Create a resource group
resource "azurerm_resource_group" "rg" {
  name     = "${local.name}-rg"
  location = local.region
}

# Create storage account
resource "azurerm_storage_account" "storage" {
  name                     = "evtmgt${random_id.server.hex}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = local.common_tags
}

resource "azurerm_storage_container" "storage_event_container" {
  name                  = "gab-events"
  storage_account_name  = azurerm_storage_account.storage.name
  container_access_type = "private"
}


# Create a linux app service plan
resource "azurerm_app_service_plan" "dev" {
  name                = "${local.name}-plan-dev"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  kind                = "Linux"
  reserved            = true

  sku {
    tier = "Basic"
    size = "B1"
  }

  tags = local.common_tags
}

# Create application insights
resource "azurerm_application_insights" "devappinsights" {
  name                = "${local.name}-dev"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"

  tags = local.common_tags
}

resource "azurerm_app_service" "devapp" {
  name                = "${local.name}-dev"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  app_service_plan_id = azurerm_app_service_plan.dev.id

  tags = local.common_tags

  site_config {
    always_on        = true
    linux_fx_version = "DOTNETCORE|5.0"
    app_command_line = "dotnet EventManagement.Web.dll"
  }

  app_settings = {
    "EVENT_CONTAINER"                = azurerm_storage_container.storage_event_container.name
    "EVENT_FILE"                     = "ga-greece-2021.json"
    "Sessionize__EventId"            = "v6jeq23c"
    "WEBSITE_RUN_FROM_PACKAGE"       = 1
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.devappinsights.instrumentation_key
  }

  connection_string {
    name  = "StorageAccount"
    type  = "Custom"
    value = azurerm_storage_account.storage.primary_connection_string
  }
}
