using System;

namespace TaskManagementSystem.ViewModels
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string UserProfileImage { get; set; } = "/images/default-avatar.svg";
        public int TaskId { get; set; }
    }
}
