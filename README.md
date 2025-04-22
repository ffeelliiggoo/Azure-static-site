# Static site hosted in Azure Storage

This project showcases a static website hosted on Azure Storage using serverless architecture. It features an Azure Function in C# that processes web requests and connects to Cosmos DB for data management, with user authentication handled through Azure Identity. The setup enables fast, scalable performance without server maintenance. 

![Cloud Resume Site architecture](https://github.com/user-attachments/assets/fc7b1e6e-0fd2-4cf5-84d2-35425d1094b5)

Below, I would like to acknowledge and give credit, in ascending order, to the main videos I followed to integrate each part of the site.

#1 ACG Projects: Build Your Resume on Azure with Blob Storage, Functions, CosmosDB, and GitHub Actions -> [video](https://youtu.be/ieYrBWmkfno) 

#2 Hosting a Static Website on Azure - Meetup April 2024 by Daniel Colón -> [video](https://www.youtube.com/watch?v=S921NkFFriM)

#3 Adding Custom Domain Name with CDN in Azure Storage (Static WebSite) + Domain Provider -> [video](https://www.youtube.com/watch?v=bVsmwv89vGE)

## Site Link:
[Check Out the Live Version of the Static Website!](https://www.routetothecloud.com/)

## Prerequisites
Make sure to look at these components first; otherwise, you may spend a lot of time and effort just adjusting your machine. Ensure that the proper downloads and extensions are set before starting the overall project for a better experience.

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

## Securing the Function secret
Your main.bicep file securely handles the Azure Function secret (Cosmos DB connection string) using a modern, best-practice approach that avoids hardcoding secrets in code or pipeline variables.

## 🔐 Azure Function Secret Handling (Bicep-based)

### 1. Key Vault Created
- A secure Azure Key Vault is deployed with RBAC enabled.

### 2. Cosmos DB Connection String Stored
- The connection string is retrieved from Cosmos DB and saved as a secret (`CosmosDbConnectionString`) in Key Vault.

### 3. Access Granted to Function App
- The Function App's **system-assigned managed identity** is granted the `Key Vault Secrets User` role.

### 4. App Setting Uses Key Vault Reference
- The Function App config uses a **Key Vault reference**, not the actual secret:

### 5. Runtime Access in Code
- The app reads the secret at runtime using:
```csharp
var connStr = Environment.GetEnvironmentVariable("CosmosDbConnectionString");


✅ Security Advantages
- 🔐 Secrets are never exposed in code or pipelines.
- 🔄 Secrets can be rotated in Key Vault without redeploying the app.
- 🔍 Access is controlled via RBAC, with a clear audit trail.

📌 Summary
![image](https://github.com/user-attachments/assets/27f07b1e-a4ab-4f75-93ab-f000f10f930f)

## Implementing Azure DevOps
This pipeline automates the build and deployment process for a web application consisting of two main parts: a backend Azure Function (written in .NET) and a static frontend hosted in Azure Blob Storage. The pipeline runs when changes are pushed to the `master` branch.

### 🔹 **Pipeline Breakdown**

#### 🔸 Variables
The pipeline starts by defining several variables:
- **`azureSubscription`**: The service connection used to authenticate and deploy resources in Azure.
- **`functionAppName`**: The name of the Azure Function App where the backend will be deployed.
- **`vmImageName`**: The type of virtual machine image used by the agent (in this case, `windows-latest`).
- **`workingDirectory`**: The path to the backend Azure Function project.
- **`blobStorageAccount`** and **`blobContainerName`**: Where the frontend static files will be uploaded.
- **`frontendDirectory`**: The path to the frontend application files.

### 🔹 Stage 1: Build

This stage handles compiling the backend and uploading the frontend.

1. Build .NET Project  
   The backend (Azure Function) is built using the `DotNetCoreCLI` task. The compiled output is saved in a `publish_output` directory.

2. Archive Build Output  
   The compiled files are zipped using the `ArchiveFiles` task, making them ready for deployment.

3. Deploy Frontend to Blob Storage  
   The frontend files are uploaded to Azure Blob Storage using the `AzureCLI` task, which runs a PowerShell script with the `az storage blob upload-batch` command.

4. Publish Build Artifact  
   The zipped backend output is saved as a build artifact named `drop` so it can be used in the next stage.

🔹 **Stage 2: Deploy**

This stage deploys the backend Azure Function.

1. **Deployment to Azure Function App**  
   The previously created `.zip` file is deployed to the Azure Function App (`fn6ic`) using the `AzureFunctionApp` task. The deployment only proceeds if the build stage succeeds.

- The **frontend** is hosted in Azure Blob Storage and automatically updated with each commit.
- The **backend** Azure Function is rebuilt and redeployed using serverless deployment via zip package.
