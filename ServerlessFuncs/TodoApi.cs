using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using ServerlessFuncs.Models;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ServerlessFuncs
{
    public static class TodoApi
    {
        private static readonly List<Todo> items = new List<Todo>();

        [FunctionName("CreateTodo")]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = "todo")]HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating a new todo item.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TodoCreate>(requestBody);

            var todo = new Todo { Description = input.Description };

            items.Add(todo);

            return new OkObjectResult(todo);
        }

        [FunctionName("GetTodos")]
        public static IActionResult GetAllTodos(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "todo")]HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Getting all todo items.");

            return new OkObjectResult(items);
        }

        [FunctionName("GetTodoById")]
        public static IActionResult GetTodoById(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "get",
                Route = "todo/{id}")]HttpRequest req,
            ILogger log,
            string id)
        {
            log.LogInformation("Getting todo item with id {0}.", id);

            var todo = items.FirstOrDefault(x => x.Id == id);
            if (todo == null)
            {
                log.LogWarning("Todo item with id '{0}' does not exist.", id);

                return new NotFoundResult();
            }

            return new OkObjectResult(todo);
        }

        [FunctionName("UpdateTodo")]
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "put",
                Route = "todo/{id}")]HttpRequest req,
            ILogger log,
            string id)
        {
            log.LogInformation("Updating todo item with id={0}.", id);

            var todo = items.FirstOrDefault(x => x.Id == id);
            if (todo == null)
            {
                log.LogWarning("Todo item with id '{0}' does not exist.", id);

                return new NotFoundResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<TodoUpdate>(requestBody);

            todo.Completed = input.Completed;
            if (!string.IsNullOrWhiteSpace(input.Description))
            {
                todo.Description = input.Description;
            }

            return new OkObjectResult(todo);
        }

        [FunctionName("DeleteTodo")]
        public static IActionResult DeleteTodo(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "delete",
                Route = "todo/{id}")]HttpRequest req,
            ILogger log,
            string id)
        {
            log.LogInformation("Deleting todo item with id {0}.", id);

            var todo = items.FirstOrDefault(x => x.Id == id);
            if (todo == null)
            {
                log.LogWarning("Todo item with id '{0}' does not exist.", id);

                return new NotFoundResult();
            }

            items.Remove(todo);

            return new OkResult();
        }
    }
}
