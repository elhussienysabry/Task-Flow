using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class Task
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public TaskStatus Status { get; set; }

        public TaskPriority Priority { get; set; }

        public DateTime? Deadline { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; } = null!;

        public string? AssignedUserId { get; set; }
        public virtual ApplicationUser? AssignedUser { get; set; }

        public int? SprintBoardId { get; set; }
        public virtual SprintBoard? SprintBoard { get; set; }

        
        public int StoryPoints { get; set; } = 0;

   
        public int? EstimatedHours { get; set; }
        public int? ActualHours { get; set; }


        public int? ParentTaskId { get; set; }
        public virtual Task? ParentTask { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        
        public int OrderInBoard { get; set; } = 0; 


        public string? Labels { get; set; }  
        public DateTime? StartDate { get; set; }  
        public int? TimeEstimate { get; set; }  
        public int? TimeSpent { get; set; }  

        public int Progress { get; set; } = 0; 
        public string? Environment { get; set; }  

       
        public string? Component { get; set; }  

        public virtual ICollection<TaskWatcher> Watchers { get; set; } = new List<TaskWatcher>();

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<TaskAttachment> Attachments { get; set; } = new List<TaskAttachment>();
        public virtual ICollection<TaskAISuggestion> AISuggestions { get; set; } = new List<TaskAISuggestion>();
        public virtual ICollection<Task> SubTasks { get; set; } = new List<Task>();
    }
}

