using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos;

namespace MyDotNetFunctionApp
{
    public class InvocationCounter
    {
        public string id { get; set; }
        public int count { get; set; }
    }

    public class CountResponse
    {
        public int count { get; set; }
        public string id { get; set; }

    }

    public class TestHttp
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _cosmosContainer;
        private const string CounterId = "functionCounter";
        private const string DatabaseName = "myDatabase";
        private const string ContainerName = "myContainer";

        public TestHttp()
        {
            string connectionString = Environment.GetEnvironmentVariable("CosmosDbConnectionString");
            _cosmosClient = new CosmosClient(connectionString);
            _cosmosContainer = _cosmosClient.GetContainer(DatabaseName, ContainerName);
        }

        private async Task EnsureCosmosResourcesExistAsync(ILogger log)
        {
            try
            {
                // Create database if it doesn't exist
                DatabaseResponse database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseName);
                log.LogInformation($"Created or verified database {DatabaseName}");

                // Create container if it doesn't exist
                ContainerProperties containerProperties = new ContainerProperties(ContainerName, "/id");
                ContainerResponse containerResponse = await database.Database.CreateContainerIfNotExistsAsync(containerProperties);
                log.LogInformation($"Created or verified container {ContainerName}");
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to ensure Cosmos resources exist: {ex.Message}");
                throw;
            }
        }

        [FunctionName("Counter")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                // Ensure Cosmos resources exist
                await EnsureCosmosResourcesExistAsync(log);

                // Read or create the counter document
                InvocationCounter counter;
                try
                {
                    var response = await _cosmosContainer.ReadItemAsync<InvocationCounter>(
                        CounterId,
                        new PartitionKey(CounterId)
                    );
                    counter = response.Resource;
                    counter.count++;
                    log.LogInformation($"Current count retrieved and incremented: {counter.count}");
                }
                catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    counter = new InvocationCounter
                    {
                        id = CounterId,
                        count = 1
                    };
                    log.LogInformation("Counter not found. Creating a new counter with count = 1.");
                }

                // Upsert the counter document
                await _cosmosContainer.UpsertItemAsync(counter, new PartitionKey(counter.id));
                log.LogInformation("Counter document upserted successfully.");

                return new OkObjectResult(new CountResponse { count = counter.count, id = counter.id });
            }
            catch (CosmosException ex)
            {
                log.LogError($"Cosmos DB error: {ex.StatusCode} - {ex.Message}");
                return new StatusCodeResult((int)ex.StatusCode);
            }
            catch (Exception ex)
            {
                log.LogError($"General error in function: {ex.Message}");
                log.LogError($"Stack trace: {ex.StackTrace}");
                return new StatusCodeResult(500);
            }
        }
    }
}