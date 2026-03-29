using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Required]
        public string ActionType { get; set; } = string.Empty; // Created, Updated, Deleted, Commented

        [Required]
        public string Description { get; set; } = string.Empty;

        public int? TaskId { get; set; }
        public virtual Task? Task { get; set; }

        public int? ProjectId { get; set; }
        public virtual Project? Project { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
