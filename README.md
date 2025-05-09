# Static site hosted in Azure Storage

This project showcases a static website hosted on Azure Storage using serverless architecture. 
It features an Azure Function in C# that processes web requests and connects to Cosmos DB for data management, with user authentication handled through Azure Identity. 
The setup enables fast, scalable performance without server maintenance. 

![Cloud Resume Site architecture](https://github.com/user-attachments/assets/fc7b1e6e-0fd2-4cf5-84d2-35425d1094b5)

Below, I would like to acknowledge and give credit, in ascending order, to the main videos I followed to integrate each part of the site.

#1 ACG Projects: Build Your Resume on Azure with Blob Storage, Functions, CosmosDB, and GitHub Actions -> [video](https://youtu.be/ieYrBWmkfno) 

#2 Hosting a Static Website on Azure - Meetup April 2024 by Daniel Colón -> [video](https://www.youtube.com/watch?v=S921NkFFriM)

#3 Adding Custom Domain Name with CDN in Azure Storage (Static WebSite) + Domain Provider -> [video](https://www.youtube.com/watch?v=bVsmwv89vGE)

## Site Link:
[Check Out the Live Version of the Static Website!](https://www.routetothecloud.com/)
https://www.routetothecloud.com/

## Prerequisites
Make sure to look at these components first; otherwise, you may spend a lot of time and effort just adjusting your machine. 
Ensure that the proper downloads and extensions are set before starting the overall project for a better experience.

- [GitHub account](https://github.com/join)
- [Azure account](https://azure.microsoft.com/en-us/free)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
- [.NET Core 3.1 LTS](https://dotnet.microsoft.com/download/dotnet/3.1)
- [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=macos%2Ccsharp%2Cbash#install-the-azure-functions-core-tools)
- [Visual Studio Code](https://code.visualstudio.com)
- [Visual Code Extensions](https://code.visualstudio.com/docs/introvideos/extend)
  - [Azure Functions Extensions](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions)
  - [C# Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
  - [Azure Storage Extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurestorage)
- [Full solution](https://github.com/ACloudGuru-Resources/acg-project-azure-resume)

## Front-end resources
The front-end is a static website built using HTML, CSS, and JavaScript. Despite being static, it includes a dynamic feature—a visitor counter. The counter’s data is retrieved through an API call to an Azure Function, enabling real-time updates on visitor traffic.

- This [article](https://www.digitalocean.com/community/tutorials/how-to-use-the-javascript-fetch-api-to-get-data) explains how to make an API call with JavaScript and in a simple way how to use it to make an API call.
- [Azure storage explorer](https://azure.microsoft.com/en-us/features/storage-explorer/) is a handy tool to use when working with Storage Accounts
- This is how you can [deploy static site to blob storage.](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-static-website-host)
  
## Back-end resources

The back-end is powered by an [HTTP triggered Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook-trigger?tabs=csharp) Azure Function, integrated with Cosmos DB using both input and output bindings. When the function is triggered, it retrieves an item from Cosmos DB, increments its value by 1, updates the database, and returns the updated value to the caller.

- [Prerequisites for developing functions with visual code locally.](https://docs.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-csharp)
- [Create a Cosmos DB account via command line](https://azure.microsoft.com/en-us/resources/templates/101-cosmosdb-free/) or [from the portal](https://docs.microsoft.com/en-us/azure/cosmos-db/create-cosmosdb-resources-portal).
- [Create an HTTP triggered Azure Function in Visual Studio Code.](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code?tabs=csharp)
- [Azure Functions Cosmos DB bindings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2)
- [Retrieve a Cosmos DB item with Functions binding.](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2-input?tabs=csharp)
- [Write to a Cosmos DB item with Functions binding.](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-cosmosdb-v2-output?tabs=csharp)
- You'll have to [enable CORS with Azure Functions](https://github.com/Azure/azure-functions-host/issues/1012) locally and once it's [deployed to Azure](https://docs.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings?tabs=portal#cors) for you website to be able to call it.

# Securing Azure Function Secrets

The `main.bicep` file in the infrastructure folder handles Azure Function secrets—specifically the Cosmos DB connection string—using secure, recommended practices. This avoids hardcoding sensitive data in code or pipeline variables.

## Secret Handling Workflow

1. **Key Vault Provisioning**  
   - An Azure Key Vault is deployed with RBAC enabled for secure access management.
  
![Keyvault secret](https://github.com/user-attachments/assets/95ac04de-0487-4e1e-b0fc-a76014c8702e)

3. **Secret Storage**  
   - The Cosmos DB connection string is retrieved and stored in Key Vault as a secret named `CosmosDbConnectionString`.
![Azure cosmos db key](https://github.com/user-attachments/assets/0fe45440-da8c-4621-b268-47910a0f5276)

4. **Identity-Based Access**  
   - The Azure Function App is assigned a system-managed identity.
   - This identity is granted the `Key Vault Secrets User` role to enable secure secret retrieval.

5. **Configuration Using Key Vault Reference**  
   - The application setting references the secret from Key Vault, rather than storing it directly:
     ```bicep
     CosmosDbConnectionString: '@Microsoft.KeyVault(VaultName=${keyVaultName};SecretName=CosmosDbConnectionString)'
     ```
6. **Runtime Access in Application Code**  
   - At runtime, the Azure Function accesses the secret using:
     ```csharp
     var connStr = Environment.GetEnvironmentVariable("CosmosDbConnectionString");
     ```
## Security Benefits

- Secrets are never exposed in source code or pipelines.  
- Secrets can be rotated in Key Vault without redeploying the application.  
- Access is managed with RBAC and logged for auditing.

