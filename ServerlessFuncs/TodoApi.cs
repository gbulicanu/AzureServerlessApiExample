using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using ServerlessFuncs.Models;

using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServerlessFuncs
{
    public static class TodoApi
    {
        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "todo")]HttpRequest request,
            [Table("todos", Connection = "AzureWebJobsStorage")]
            IAsyncCollector<TodoTableEntity> todoTable,
            [Queue("todos", Connection = "AzureWebJobsStorage")]
            IAsyncCollector<Todo> todoQueue,
            ILogger log)
        {
            log.LogInformation("Creating a new todo item.");

            string requestBody = await new StreamReader(request.Body)
                .ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TodoCreate>(requestBody);

            var todo = new Todo { Description = input.Description };

            await todoTable.AddAsync(todo.ToTableEntity());
            await todoQueue.AddAsync(todo);

            return new OkObjectResult(todo);
        }

        [FunctionName("GetTodos")]
        public static async Task<IActionResult> GetAllTodos(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "todo")]
            HttpRequest request,
            [Table("todos", Connection = "AzureWebJobsStorage")]
            CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("Getting all todo items.");
            var query = new TableQuery<TodoTableEntity>();
            var segment = await todoTable.ExecuteQuerySegmentedAsync(
                query,
                null);

            return new OkObjectResult(segment.Select(Mappings.ToTodo));
        }

        [FunctionName("GetTodoById")]
        public static IActionResult GetTodoById(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "todo/{id}")]HttpRequest request,
            [Table(
                "todos",
                "TODO",
                "{id}",
                Connection = "AzureWebJobsStorage")]
            TodoTableEntity todo,
            ILogger log,
            string id)
        {
            log.LogInformation("Getting todo item with id {0}.", id);

            if (todo == null)
            {
                log.LogWarning("Todo item with id '{0}' does not exist.", id);

                return new NotFoundResult();
            }

            return new OkObjectResult(todo.ToTodo());
        }

        [FunctionName("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "put",
                Route = "todo/{id}")]
            HttpRequest request,
            [Table("todos", Connection = "AzureWebJobsStorage")]
            CloudTable todoTable,
            ILogger log,
            string id)
        {
            log.LogInformation("Updating todo item with id={0}.", id);

            string requestBody = await new StreamReader(request.Body)
                .ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TodoUpdate>(requestBody);

            var findOperation = TableOperation.Retrieve(
                "TODO",
                id);
            var findResult = await todoTable.ExecuteAsync(findOperation);

            if (findResult.HttpStatusCode == 404)
            {
                log.LogWarning("Todo item with id '{0}' does not exist.", id);

                return new NotFoundResult();
            }

            var existingRow = (DynamicTableEntity)findResult.Result;
            var exitingEntity = TableEntity.ConvertBack<TodoTableEntity>(
                existingRow.Properties,
                new OperationContext());
            exitingEntity.ETag = "*";
            exitingEntity.PartitionKey = "TODO";
            exitingEntity.RowKey = existingRow.RowKey;
            exitingEntity.Completed = input.Completed;
            if (!string.IsNullOrWhiteSpace(input.Description))
            {
                exitingEntity.Description = input.Description;
            }

            var replaceOperation = TableOperation.Replace(exitingEntity);
            await todoTable.ExecuteAsync(replaceOperation);

            return new OkObjectResult(exitingEntity.ToTodo());
        }

        [FunctionName("DeleteTodo")]
        public static async Task<IActionResult> DeleteTodo(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "delete",
                Route = "todo/{id}")]
            HttpRequest request,
            [Table("todos", Connection = "AzureWebJobsStorage")]
            CloudTable todoTable,
            ILogger log,
            string id)
        {
            log.LogInformation("Deleting todo item with id {0}.", id);

            var deleteOperation = TableOperation.Delete(new TableEntity
            {
                PartitionKey = "TODO",
                RowKey = id,
                ETag = "*"
            });

            try
            {
                var deleteResult =
                    await todoTable.ExecuteAsync(deleteOperation);
            }
            catch (StorageException e)
            when (e.RequestInformation.HttpStatusCode == 404)
            {
                log.LogWarning("Todo item with id '{0}' does not exist.", id);

                return new NotFoundResult();
            }

            return new OkResult();
        }
    }
}
