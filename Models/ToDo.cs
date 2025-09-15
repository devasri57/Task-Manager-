using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ToDo.Models
{
    public class ToDo
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a description.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a due date.")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        [ValidateNever]
        public Category Category { get; set; } = null!;

        [Required(ErrorMessage = "Please select a status.")]
        public int StatusId { get; set; }

        [ValidateNever]
        public Status Status { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;

        // New fields for tracking task performance
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? CompletedAt { get; set; }

        // Computed property to check overdue tasks
        public bool Overdue =>
            StatusId == 1 &&
            DueDate.HasValue &&
            DueDate.Value.Date < DateTime.Today;

        // Computed property for time 
        public int? TimeTakenDays =>
     CompletedAt.HasValue ? (int)(CompletedAt.Value - CreatedAt).TotalDays : (int?)null;

    }
}
