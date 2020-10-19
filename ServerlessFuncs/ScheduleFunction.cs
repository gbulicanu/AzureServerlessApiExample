using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using ServerlessFuncs.Models;

using System;
using System.Threading.Tasks;

namespace ServerlessFuncs
{
    public static class ScheduleFunction
    {
        [FunctionName("ScheduleFunction")]
        public static async Task Run(
            [TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
            [Table("todos", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            var query = new TableQuery<TodoTableEntity>();
            var segment = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            var deleted = 0;
            foreach (var todo in segment)
            {
                if (todo.Completed)
                {
                    await todoTable.ExecuteAsync(TableOperation.Delete(todo));
                    deleted++;
                }
            }
            log.LogInformation($"Deleted {deleted} completed todo's at: {DateTime.Now}");
        }
    }
}
