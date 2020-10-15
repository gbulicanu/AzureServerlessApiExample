using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using ServerlessFuncs.Models;

using System.Collections.Generic;
using System.IO;
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
    }
}
