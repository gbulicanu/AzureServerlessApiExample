using Microsoft.WindowsAzure.Storage.Table;

using System;

namespace ServerlessFuncs.Models
{
    public class Todo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("n");

        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public string Description { get; set; }

        public bool Completed { get; set; }
    }

    public class TodoCreate
    {
        public string Description { get; set; }
    }

    public class TodoUpdate
    {
        public bool Completed { get; set; }

        public string Description { get; set; }
    }

    public class TodoTableEntity : TableEntity
    {
        public DateTime CreatedTime { get; set; }

        public string Description { get; set; }

        public bool Completed { get; set; }
    }

    public static class Mappings
    {
        public static TodoTableEntity ToTableEntity(this Todo source)
        {
            return new TodoTableEntity
            {
                PartitionKey = "TODO",
                RowKey = source.Id,
                CreatedTime = source.CreatedTime,
                Completed = source.Completed,
                Description = source.Description
            };
        }

        public static Todo ToTodo(this TodoTableEntity source)
        {
            return new Todo
            {
                Id = source.PartitionKey,
                CreatedTime = source.CreatedTime,
                Completed = source.Completed,
                Description = source.Description
            };
        }
    }
}
