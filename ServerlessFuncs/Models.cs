using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
