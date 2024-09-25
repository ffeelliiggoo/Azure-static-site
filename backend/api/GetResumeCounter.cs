using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Company.Function.CosmosDB
{
    public static class GetResumeCounter
    {
        [FunctionName("GetResumeCounter")]
        public static async Task<HttpResponseMessage> Run(  // Mark the method as async and change return type to Task<HttpResponseMessage>
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req,
            [CosmosDB(databaseName: "AzureResume",containerName: "Counter", Connection = "AzureResumeConnectionString", Id = "1",
            PartitionKey = "id")] Counter counter,
            [CosmosDB( databaseName: "AzureResume", containerName: "Counter", Connection = "AzureResumeConnectionString",
            PartitionKey = "id")] IAsyncCollector<Counter> outputCounter,  // Output binding to save the updated counter
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Check if the counter object is null (document not found)
            if (counter == null)
            {
                log.LogInformation("Document not found, creating a new one.");
                counter = new Counter { Id = "1", Count = 1 };  // Initialize a new Counter document
            }
            else
            {
                log.LogInformation($"Document found, current count: {counter.Count}");
                counter.Count += 1;  // Increment the counter
            }

            // Save the updated or new counter back to Cosmos DB
            await outputCounter.AddAsync(counter);

            // Serialize the updated counter to JSON
            var jsonToReturn = JsonConvert.SerializeObject(counter);

            // Return the updated counter in the response
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
            };
        }
    }

    public class Counter
    {
        public string Id { get; set; }
        public int Count { get; set; }
    }
}