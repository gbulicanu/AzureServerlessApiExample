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
                return new NotFoundResult();
            }

            return new OkObjectResult(todo);
        }
    }
}
