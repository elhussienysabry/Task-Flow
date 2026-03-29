using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using Task = TaskManagementSystem.Models.Task;

namespace TaskManagementSystem.Services
{
    public interface INotificationService
    {
        System.Threading.Tasks.Task CreateNotificationAsync(string userId, string message, NotificationType type, string? description = null, int? taskId = null, int? projectId = null, string? actionUrl = null, string? triggeredByUserId = null);
        System.Threading.Tasks.Task<List<Notification>> GetUserNotificationsAsync(string userId, int page = 1, int pageSize = 20);
        System.Threading.Tasks.Task<int> GetUnreadCountAsync(string userId);
        System.Threading.Tasks.Task MarkAsReadAsync(int notificationId);
        System.Threading.Tasks.Task MarkAllAsReadAsync(string userId);
        System.Threading.Tasks.Task DeleteNotificationAsync(int notificationId);
        System.Threading.Tasks.Task NotifyTaskAssignmentAsync(Task task, string assignedUserId, string triggeredByUserId);
        System.Threading.Tasks.Task NotifyTaskCompletionAsync(Task task, string triggeredByUserId);
        System.Threading.Tasks.Task NotifyTaskUpdateAsync(Task task, string triggeredByUserId);
        System.Threading.Tasks.Task NotifyDeadlineReminderAsync(Task task);
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task CreateNotificationAsync(
            string userId, 
            string message, 
            NotificationType type, 
            string? description = null, 
            int? taskId = null, 
            int? projectId = null, 
            string? actionUrl = null,
            string? triggeredByUserId = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Type = type,
                Description = description,
                RelatedTaskId = taskId,
                RelatedProjectId = projectId,
                ActionUrl = actionUrl,
                TriggeredByUserId = triggeredByUserId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task<List<Notification>> GetUserNotificationsAsync(string userId, int page = 1, int pageSize = 20)
        {
            return await _context.Notifications
                .Include(n => n.RelatedTask)
                .Include(n => n.RelatedProject)
                .Include(n => n.TriggeredByUser)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async System.Threading.Tasks.Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteNotificationAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task NotifyTaskAssignmentAsync(Task task, string assignedUserId, string triggeredByUserId)
        {
            var assignedUser = await _context.Users.FindAsync(assignedUserId);
            if (assignedUser != null)
            {
                var message = $"تم إسناد لك مهمة: {task.Title}";
                var actionUrl = $"/Tasks/Details/{task.Id}";
                
                await CreateNotificationAsync(
                    userId: assignedUserId,
                    message: message,
                    type: NotificationType.TaskAssigned,
                    description: task.Description,
                    taskId: task.Id,
                    projectId: task.ProjectId,
                    actionUrl: actionUrl,
                    triggeredByUserId: triggeredByUserId
                );
            }
        }

        public async System.Threading.Tasks.Task NotifyTaskCompletionAsync(Task task, string triggeredByUserId)
        {
            var project = await _context.Projects.FindAsync(task.ProjectId);
            if (project != null)
            {
                var projectOwner = await _context.Users.FindAsync(project.OwnerId);
                if (projectOwner != null)
                {
                    var message = $"تم إكمال المهمة: {task.Title}";
                    var actionUrl = $"/Tasks/Details/{task.Id}";
                    
                    await CreateNotificationAsync(
                        userId: projectOwner.Id,
                        message: message,
                        type: NotificationType.TaskCompleted,
                        taskId: task.Id,
                        projectId: task.ProjectId,
                        actionUrl: actionUrl,
                        triggeredByUserId: triggeredByUserId
                    );
                }
            }
        }

        public async System.Threading.Tasks.Task NotifyTaskUpdateAsync(Task task, string triggeredByUserId)
        {
            if (!string.IsNullOrEmpty(task.AssignedUserId) && task.AssignedUserId != triggeredByUserId)
            {
                var message = $"تم تحديث المهمة: {task.Title}";
                var actionUrl = $"/Tasks/Details/{task.Id}";
                
                await CreateNotificationAsync(
                    userId: task.AssignedUserId,
                    message: message,
                    type: NotificationType.TaskUpdated,
                    taskId: task.Id,
                    projectId: task.ProjectId,
                    actionUrl: actionUrl,
                    triggeredByUserId: triggeredByUserId
                );
            }
        }

        public async System.Threading.Tasks.Task NotifyDeadlineReminderAsync(Task task)
        {
            if (!string.IsNullOrEmpty(task.AssignedUserId) && task.Deadline.HasValue)
            {
                var daysUntilDeadline = (task.Deadline.Value - DateTime.UtcNow).TotalDays;
                if (daysUntilDeadline <= 1 && daysUntilDeadline > 0)
                {
                    var message = $"تذكير: الموعد النهائي للمهمة '{task.Title}' غداً";
                    var actionUrl = $"/Tasks/Details/{task.Id}";
                    
                    await CreateNotificationAsync(
                        userId: task.AssignedUserId,
                        message: message,
                        type: NotificationType.TaskDeadlineReminder,
                        taskId: task.Id,
                        projectId: task.ProjectId,
                        actionUrl: actionUrl
                    );
                }
            }
        }
    }
}
