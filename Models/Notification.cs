using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        public NotificationType Type { get; set; } = NotificationType.General;

        // الكائن المرتبط بالتنبيه
        public int? RelatedTaskId { get; set; }
        
        [ForeignKey("RelatedTaskId")]
        public virtual Task? RelatedTask { get; set; }

        public int? RelatedProjectId { get; set; }
        
        [ForeignKey("RelatedProjectId")]
        public virtual Project? RelatedProject { get; set; }

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReadAt { get; set; }

        // Action URL
        public string? ActionUrl { get; set; }

        // التنبيهات من الأشخاص
        public string? TriggeredByUserId { get; set; }
        
        [ForeignKey("TriggeredByUserId")]
        public virtual ApplicationUser? TriggeredByUser { get; set; }
    }

    public enum NotificationType
    {
        General = 0,
        TaskAssigned = 1,
        TaskCompleted = 2,
        TaskUpdated = 3,
        CommentAdded = 4,
        ProjectInvitation = 5,
        TaskDeadlineReminder = 6,
        TaskPriorityChanged = 7,
        Mention = 8,
        SubTaskCreated = 9
    }
}
