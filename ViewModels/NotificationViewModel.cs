using System;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.ViewModels
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string? ActionUrl { get; set; }
        public int? RelatedTaskId { get; set; }
        public int? RelatedProjectId { get; set; }
        public string? TriggeredByUserName { get; set; }
        public string? TriggeredByUserImage { get; set; }
    }

    public class NotificationResponseViewModel
    {
        public int Unread { get; set; }
        public int Total { get; set; }
        public List<NotificationViewModel> Notifications { get; set; } = new();
    }
}
