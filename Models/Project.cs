using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public string OwnerId { get; set; } = string.Empty;
        public virtual ApplicationUser Owner { get; set; } = null!;

        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
        public virtual ICollection<ProjectCollaborator> Collaborators { get; set; } = new List<ProjectCollaborator>();
    }
}
