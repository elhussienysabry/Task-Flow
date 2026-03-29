using System;
using System.ComponentModel.DataAnnotations;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.ViewModels
{
    public class ProjectViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string? OwnerName { get; set; }
        public int TaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
        public double CompletionPercentage { get; set; }
    }
}
