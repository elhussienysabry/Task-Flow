using System;

namespace TaskManagementSystem.Models
{
    public class TaskAISuggestion
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string SuggestionType { get; set; } = string.Empty; // Priority, Status, Assignee, Duration
        public string SuggestedValue { get; set; } = string.Empty;
        public double ConfidenceScore { get; set; } // 0-100
        public string Reason { get; set; } = string.Empty;
        public bool IsApplied { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Task? Task { get; set; }
    }
}
