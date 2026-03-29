using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Models;
using Task = System.Threading.Tasks.Task;
using TaskModel = TaskManagementSystem.Models.Task;

namespace TaskManagementSystem.Services
{
    public interface IAIService
    {
        System.Threading.Tasks.Task<List<TaskAISuggestion>> GetTaskSuggestionsAsync(TaskModel task);
        System.Threading.Tasks.Task<string> GetTaskPrioritySuggestionAsync(TaskModel task);
        System.Threading.Tasks.Task<string> GetTaskStatusSuggestionAsync(TaskModel task);
        System.Threading.Tasks.Task<int> GetEstimatedHoursAsync(TaskModel task);
        double CalculateTaskCompletionProbability(TaskModel task);
        string GetAISummaryAsync(TaskModel task);
    }

    public class AIService : IAIService
    {
        /// <summary>
        /// يحصل على اقتراحات ذكية للمهمة بناءً على البيانات التاريخية
        /// </summary>
        public async System.Threading.Tasks.Task<List<TaskAISuggestion>> GetTaskSuggestionsAsync(TaskModel task)
        {
            var suggestions = new List<TaskAISuggestion>();

            // اقتراح الأولوية
            var prioritySuggestion = await GetTaskPrioritySuggestionAsync(task);
            if (!string.IsNullOrEmpty(prioritySuggestion))
            {
                suggestions.Add(new TaskAISuggestion
                {
                    SuggestionType = "Priority",
                    SuggestedValue = prioritySuggestion,
                    ConfidenceScore = CalculatePriorityConfidence(task),
                    Reason = $"Based on task complexity and deadline, priority should be {prioritySuggestion}"
                });
            }

            // اقتراح المدة الزمنية
            var estimatedHours = await GetEstimatedHoursAsync(task);
            if (estimatedHours > 0)
            {
                suggestions.Add(new TaskAISuggestion
                {
                    SuggestionType = "Duration",
                    SuggestedValue = $"{estimatedHours} hours",
                    ConfidenceScore = CalculateDurationConfidence(task),
                    Reason = $"Similar tasks typically take {estimatedHours} hours to complete"
                });
            }

            return await System.Threading.Tasks.Task.FromResult(suggestions);
        }

        public async System.Threading.Tasks.Task<string> GetTaskPrioritySuggestionAsync(TaskModel task)
        {
            // منطق ذكي لتحديد الأولوية بناءً على الموعد النهائي والمحتوى
            if (task.Deadline.HasValue)
            {
                var daysUntilDeadline = (task.Deadline.Value - DateTime.Now).Days;
                
                if (daysUntilDeadline <= 1)
                    return await System.Threading.Tasks.Task.FromResult("High");
                else if (daysUntilDeadline <= 3)
                    return await System.Threading.Tasks.Task.FromResult("High");
                else if (daysUntilDeadline <= 7)
                    return await System.Threading.Tasks.Task.FromResult("Medium");
                else
                    return await System.Threading.Tasks.Task.FromResult("Low");
            }

            // التحليل على أساس العنوان والوصف
            var contentLower = (task.Title + " " + (task.Description ?? "")).ToLower();
            if (contentLower.Contains("urgent") || contentLower.Contains("critical") || contentLower.Contains("asap"))
                return await System.Threading.Tasks.Task.FromResult("High");
            
            if (contentLower.Contains("bug") || contentLower.Contains("fix"))
                return await System.Threading.Tasks.Task.FromResult("High");

            return await System.Threading.Tasks.Task.FromResult("Medium");
        }

        public async System.Threading.Tasks.Task<string> GetTaskStatusSuggestionAsync(TaskModel task)
        {
            // اقتراح الحالة بناءً على المدة الزمنية والموعد
            if (task.EstimatedHours.HasValue && task.ActualHours.HasValue)
            {
                if (task.ActualHours >= task.EstimatedHours * 0.9)
                    return await System.Threading.Tasks.Task.FromResult("In Progress");
            }

            return await System.Threading.Tasks.Task.FromResult(task.Status.ToString());
        }

        public async System.Threading.Tasks.Task<int> GetEstimatedHoursAsync(TaskModel task)
        {
            // تقدير المدة بناءً على تعقيد الوصف والأولوية
            int hours = 2; // القيمة الافتراضية

            // إذا كان هناك وصف مفصل، قد تكون المهمة أكثر تعقيداً
            if (!string.IsNullOrEmpty(task.Description))
            {
                var wordCount = task.Description.Split(' ').Length;
                hours += (wordCount / 50); // كل 50 كلمة تضيف ساعة تقريباً
            }

            // بناءً على الأولوية
            switch (task.Priority)
            {
                case TaskPriority.High:
                    hours *= 2;
                    break;
                case TaskPriority.Medium:
                    hours = (int)(hours * 1.5);
                    break;
                case TaskPriority.Low:
                    hours = Math.Max(1, hours - 1);
                    break;
            }

            return await System.Threading.Tasks.Task.FromResult(hours);
        }

        public double CalculateTaskCompletionProbability(TaskModel task)
        {
            // احسب احتمالية إكمال المهمة بناءً على عوامل مختلفة
            double probability = 50.0;

            if (task.AssignedUserId != null)
                probability += 10;

            if (task.Deadline.HasValue && task.Deadline.Value > DateTime.Now)
                probability += 15;

            if (!string.IsNullOrEmpty(task.Description))
                probability += 10;

            if (task.StoryPoints > 0)
                probability += 5;

            return Math.Min(100, probability);
        }

        public string GetAISummaryAsync(TaskModel task)
        {
            // إنشاء ملخص ذكي للمهمة
            var summary = $"📋 {task.Title}\n";
            summary += $"⏰ {(task.Deadline.HasValue ? task.Deadline.Value.ToShortDateString() : "No deadline")}\n";
            summary += $"👤 {(task.AssignedUserId != null ? "Assigned" : "Unassigned")}\n";
            summary += $"📊 Completion Probability: {CalculateTaskCompletionProbability(task)}%\n";

            return summary;
        }

        private double CalculatePriorityConfidence(TaskModel task)
        {
            double confidence = 60.0;
            if (task.Deadline.HasValue) confidence += 20;
            if (!string.IsNullOrEmpty(task.Description)) confidence += 10;
            return Math.Min(100, confidence);
        }

        private double CalculateDurationConfidence(TaskModel task)
        {
            double confidence = 50.0;
            if (!string.IsNullOrEmpty(task.Description)) confidence += 20;
            if (task.StoryPoints > 0) confidence += 15;
            return Math.Min(100, confidence);
        }
    }
}
