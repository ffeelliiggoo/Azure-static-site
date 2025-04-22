#!/bin/bash

# Exit immediately if a command exits with a non-zero status
set -e

# Configuration
LOCATION="westus"
RESOURCE_GROUP_NAME="portfolio2-rg"
ENVIRONMENT="dev"

# Resource names - using much shorter names
RANDOM_SUFFIX=$(cat /dev/urandom | LC_ALL=C tr -dc 'a-z0-9' | fold -w 3 | head -n 1)
FUNCTION_APP_NAME="fn${RANDOM_SUFFIX}"  # Much shorter base name
KEYVAULT_NAME="kv${RANDOM_SUFFIX}"
COSMOS_DB_NAME="cs${RANDOM_SUFFIX}"

# Function to clean up resources
cleanup() {
    echo "Cleaning up resources..."
    echo "Deleting resource group ${RESOURCE_GROUP_NAME}..."
    az group delete --name $RESOURCE_GROUP_NAME --yes --no-wait
    echo "Cleanup initiated. Resource group deletion is in progress."
}

# Error handler
handle_error() {
    echo "Error occurred at line $1"
    cleanup
    exit 1
}

# Set up error handling
trap 'handle_error $LINENO' ERR

echo "Using the following resource names:"
echo "Function App: $FUNCTION_APP_NAME"
echo "Key Vault: $KEYVAULT_NAME"
echo "Cosmos DB: $COSMOS_DB_NAME"

# Create Resource Group
echo "Creating Resource Group..."
az group create \
  --name $RESOURCE_GROUP_NAME \
  --location $LOCATION

# Validate Bicep template
echo "Validating Bicep template..."
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
    cleanup
    exit 1
fi

echo "Validation successful! Proceeding with deployment..."

# Check for soft-deleted Key Vault and purge if needed
echo "Checking for soft-deleted Key Vault..."
az keyvault list-deleted --query "[?name=='$KEYVAULT_NAME'].name" -o tsv | while read -r vault; do
    echo "Purging soft-deleted Key Vault: $vault"
    az keyvault purge --name $vault
done

# Wait for purge to complete
sleep 30

# Deploy Bicep template
echo "Deploying Bicep template..."
if ! az deployment group create \
  --resource-group $RESOURCE_GROUP_NAME \
  --template-file main.bicep \
  --parameters \
    functionAppName=$FUNCTION_APP_NAME \
    keyVaultName=$KEYVAULT_NAME \
    cosmosDbAccountName=$COSMOS_DB_NAME \
    environment=$ENVIRONMENT \
    location=$LOCATION; then
    echo "Deployment failed!"
    cleanup
    exit 1
fi

echo "Deployment completed successfully!"