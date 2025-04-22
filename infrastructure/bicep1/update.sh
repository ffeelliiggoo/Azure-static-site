#!/bin/bash

# Exit immediately if a command exits with a non-zero status
set -e

# Configuration
RESOURCE_GROUP_NAME="portfolio2-rg"
LOCATION="westus"
ENVIRONMENT="dev"

# Get existing resource names
echo "Getting existing resource names..."
FUNCTION_APP_NAME="fn87x"
KEYVAULT_NAME="kv87x"
COSMOS_DB_NAME="cs87x"

if [ -z "$FUNCTION_APP_NAME" ] || [ -z "$KEYVAULT_NAME" ] || [ -z "$COSMOS_DB_NAME" ]; then
    echo "Error: Could not retrieve all existing resource names"
    echo "Function App: $FUNCTION_APP_NAME"
    echo "Key Vault: $KEYVAULT_NAME"
    echo "Cosmos DB: $COSMOS_DB_NAME"
    exit 1
fi

echo "Using existing resource names:"
echo "Function App: $FUNCTION_APP_NAME"
echo "Key Vault: $KEYVAULT_NAME"
echo "Cosmos DB: $COSMOS_DB_NAME"

# Error handler
handle_error() {
    echo "Error occurred at line $1"
    exit 1
}

# Set up error handling
trap 'handle_error $LINENO' ERR

# Validate Bicep template
echo "Validating Bicep template update..."
if ! az deployment group validate \
  --resource-group $RESOURCE_GROUP_NAME \
  --template-file main.bicep \
  --parameters \
    functionAppName=$FUNCTION_APP_NAME \
    keyVaultName=$KEYVAULT_NAME \
    cosmosDbAccountName=$COSMOS_DB_NAME \
    environment=$ENVIRONMENT \
    location=$LOCATION; then
    echo "Validation failed!"
    exit 1
fi

echo "Validation successful! Proceeding with update..."

# Deploy updated Bicep template
echo "Updating resources..."
if ! az deployment group create \
  --resource-group $RESOURCE_GROUP_NAME \
  --template-file main.bicep \
  --parameters \
    functionAppName=$FUNCTION_APP_NAME \
    keyVaultName=$KEYVAULT_NAME \
    cosmosDbAccountName=$COSMOS_DB_NAME \
    environment=$ENVIRONMENT \
    location=$LOCATION \
    --mode Incremental; then
    echo "Update failed!"
    exit 1
fi

echo "Update completed successfully!"