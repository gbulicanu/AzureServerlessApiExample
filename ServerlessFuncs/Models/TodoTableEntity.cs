using Microsoft.Azure.Cosmos.Table;

using System;

namespace ServerlessFuncs.Models
{
    public class TodoTableEntity : TableEntity
    {
        public DateTime CreatedTime { get; set; }

        public string Description { get; set; }

        public bool Completed { get; set; }
    }
}
