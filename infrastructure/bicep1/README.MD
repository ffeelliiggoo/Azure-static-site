
# Azure DevOps Pipeline Overview

This pipeline automates the build and deployment process for a web application consisting of:

- A .NET-based backend Azure Function  
- A static frontend hosted in Azure Blob Storage  

The pipeline triggers automatically on pushes to the `master` branch.

## Key Pipeline Variables

- `azureSubscription`: Azure service connection used for deployment  
- `functionAppName`: Name of the Azure Function App  
- `vmImageName`: Agent VM image (e.g., `windows-latest`)  
- `workingDirectory`: Path to the backend Azure Function project  
- `blobStorageAccount` / `blobContainerName`: Target Blob Storage for frontend files  
- `frontendDirectory`: Path to static frontend assets  

## Stage 1: Build

1. **Build Backend (.NET)**  
   - Uses the `DotNetCoreCLI` task to build the Azure Function  
   - Output is stored in the `publish_output` directory

2. **Archive Output**  
   - Uses the `ArchiveFiles` task to zip the build artifacts

3. **Upload Frontend to Blob Storage**  
   - Uses the `AzureCLI` task with PowerShell to run:
     ```bash
     az storage blob upload-batch
     ```
4. **Publish Build Artifacts**  
   - The backend zip is published as a build artifact named `drop`

## Stage 2: Deploy

1. **Deploy to Azure Function App**  
   - Uses the `AzureFunctionApp` task to deploy the zip package  
   - This stage only runs if the build stage is successful

## Summary
- The frontend is hosted in Azure Blob Storage and auto-updated on every commit  
- The backend Azure Function is rebuilt and deployed using a serverless model  
- Secure practices are applied end-to-end through automation and role-based access
