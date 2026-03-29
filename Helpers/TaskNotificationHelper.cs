using System;
using System.Threading.Tasks;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;
using Task = TaskManagementSystem.Models.Task;
using TaskStatus = TaskManagementSystem.Models.TaskStatus;

namespace TaskManagementSystem.Helpers
{
    /// <summary>
    /// Helper class لتسهيل العمليات المتعلقة بالمهام والإشعارات
    /// </summary>
    public class TaskNotificationHelper
    {
        private readonly INotificationService _notificationService;

        public TaskNotificationHelper(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// إرسال إشعار عند إسناد مهمة
        /// </summary>
        public async System.Threading.Tasks.Task NotifyTaskAssignmentAsync(Task task, string newAssigneeId, string changedByUserId)
        {
            if (!string.IsNullOrEmpty(newAssigneeId))
            {
                await _notificationService.NotifyTaskAssignmentAsync(task, newAssigneeId, changedByUserId);
            }
        }

        /// <summary>
        /// إرسال إشعار عند تغيير الأولوية
        /// </summary>
        public async System.Threading.Tasks.Task NotifyPriorityChangeAsync(Task task, TaskPriority oldPriority, TaskPriority newPriority, string changedByUserId)
        {
            if (oldPriority != newPriority && !string.IsNullOrEmpty(task.AssignedUserId))
            {
                var message = $"تم تغيير أولوية المهمة '{task.Title}' من {oldPriority} إلى {newPriority}";
                await _notificationService.CreateNotificationAsync(
                    task.AssignedUserId,
                    message,
                    NotificationType.TaskPriorityChanged,
                    $"الأولوية الجديدة: {newPriority}",
                    task.Id,
                    task.ProjectId,
                    $"/Tasks/Details/{task.Id}",
                    changedByUserId
                );
            }
        }

        /// <summary>
        /// إرسال إشعار عند تحديث حالة المهمة
        /// </summary>
        public async System.Threading.Tasks.Task NotifyStatusChangeAsync(Task task, TaskStatus oldStatus, TaskStatus newStatus, string changedByUserId)
        {
            var message = $"تم تغيير حالة المهمة '{task.Title}' إلى {newStatus}";
            
            if (newStatus == Models.TaskStatus.Done)
            {
                await _notificationService.NotifyTaskCompletionAsync(task, changedByUserId);
            }
            else if (!string.IsNullOrEmpty(task.AssignedUserId) && task.AssignedUserId != changedByUserId)
            {
                await _notificationService.CreateNotificationAsync(
                    task.AssignedUserId,
                    message,
                    NotificationType.TaskUpdated,
                    $"الحالة الجديدة: {newStatus}",
                    task.Id,
                    task.ProjectId,
                    $"/Tasks/Details/{task.Id}",
                    changedByUserId
                );
            }
        }

        /// <summary>
        /// إرسال إشعار عند تغيير موعد نهائي
        /// </summary>
        public async System.Threading.Tasks.Task NotifyDeadlineChangeAsync(Task task, DateTime? oldDeadline, DateTime? newDeadline, string changedByUserId)
        {
            if (!string.IsNullOrEmpty(task.AssignedUserId) && newDeadline.HasValue)
            {
                var message = $"تم تحديث الموعد النهائي للمهمة '{task.Title}' إلى {newDeadline:dd/MM/yyyy}";
                await _notificationService.CreateNotificationAsync(
                    task.AssignedUserId,
                    message,
                    NotificationType.TaskUpdated,
                    $"الموعد الجديد: {newDeadline:dd/MM/yyyy}",
                    task.Id,
                    task.ProjectId,
                    $"/Tasks/Details/{task.Id}",
                    changedByUserId
                );
            }
        }
    }
}
