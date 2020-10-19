using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using ServerlessFuncs.Models;

using System.Threading.Tasks;

namespace ServerlessFuncs
{
    public static class QueueListeners
    {
        [FunctionName("QueueListeners")]
        public static async Task Run(
            [QueueTrigger("todos", Connection = "AzureWebJobsStorage")]Todo todo,
            [Blob("todos", Connection = "AzureWebJobsStorage")]CloudBlobContainer container,
            ILogger log)
        {
            await container.CreateIfNotExistsAsync();
            var blob = container.GetBlockBlobReference($"{todo.Id}.txt");
            await blob.UploadTextAsync($"Create a new task {todo.Description}");
            log.LogInformation($"C# Queue trigger function processed: {todo.Description}");
        }
    }
}
