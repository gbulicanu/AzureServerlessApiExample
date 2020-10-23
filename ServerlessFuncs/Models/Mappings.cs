namespace ServerlessFuncs.Models
{
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
                Id = source.RowKey,
                CreatedTime = source.CreatedTime,
                Completed = source.Completed,
                Description = source.Description
            };
        }
    }
}
