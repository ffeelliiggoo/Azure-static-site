# Felipe Gonzalez resume hosted in Azure Storage
I built this page as part of the Cloud Guru Azure Resume Challenge, leveraging Azure's powerful ecosystem to deploy a fully functional static website. By integrating Functions, Azure Storage for hosting and Cosmos DB for seamless data management. This project allowed me to dive deep into real-world cloud infrastructure design, optimize performance, and enhance my expertise in deploying secure, scalable solutions on Microsoft Azure—all while having some fun with cutting-edge tech!
![Cloud Resume Site architecture](https://github.com/user-attachments/assets/602c2501-3951-4f5e-9601-2632701cb2fb)
Below, I would like to acknowledge and give credit, in ascending order, to the main videos I followed to integrate each part of the site.

#1 ACG Projects: Build Your Resume on Azure with Blob Storage, Functions, CosmosDB, and GitHub Actions -> [video](https://youtu.be/ieYrBWmkfno) 

#2 Hosting a Static Website on Azure - Meetup April 2024 by Daniel Colón -> [video](https://www.youtube.com/watch?v=S921NkFFriM)

#3 Adding Custom Domain Name with CDN in Azure Storage (Static WebSite) + Domain Provider -> [video](https://www.youtube.com/watch?v=bVsmwv89vGE)

## Demo


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



