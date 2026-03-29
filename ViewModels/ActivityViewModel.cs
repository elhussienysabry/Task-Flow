using System;

namespace TaskManagementSystem.ViewModels
{
    public class ActivityViewModel
    {
        public int Id { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? TaskId { get; set; }
        public string? TaskTitle { get; set; }
        public int? ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
