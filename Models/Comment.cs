using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int TaskId { get; set; }
        public virtual Task Task { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        // Mention support - store mentioned user IDs
        public string MentionedUserIds { get; set; } = string.Empty; // Comma-separated IDs

        // Support for threaded comments
        public int? ParentCommentId { get; set; }
        public virtual Comment? ParentComment { get; set; }
    }
}
