using System;

namespace ToDo.Models
{
    public class DeletedTask
    {
        public int Id { get; set; }               
        public int OriginalTaskId { get; set; }   
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public string? CategoryName { get; set; }
        public string? StatusName { get; set; }
        public DateTime DeletedOn { get; set; } = DateTime.Now;
    }
}
