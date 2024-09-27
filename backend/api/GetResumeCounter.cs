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
        public static async Task<HttpResponseMessage> Run(  
            [HttpTrigger(AuthorizationLevel.Function, "get","post")] HttpRequest req,
            [CosmosDB(databaseName: "AzureResume", containerName: "Counter", Connection = "AzureResumeConnectionString", Id = "1", PartitionKey = "1")] Counter counter,
            [CosmosDB(databaseName: "AzureResume", containerName: "Counter", Connection = "AzureResumeConnectionString")] IAsyncCollector<Counter> counterCollector,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            // Increment the counter directly
            counter.Count += 1;

            // Save the updated counter back to Cosmos DB (only once)
            await counterCollector.AddAsync(counter);

            // Serialize the updated counter to JSON
            var jsonToReturn = JsonConvert.SerializeObject(counter);

            // Return the updated counter in the response
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(jsonToReturn, Encoding.UTF8, "application/json")
            };
        }
    }
}
