using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services;

namespace TaskManagementSystem.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationsController(
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        /// <summary>
        /// الحصول على إشعارات المستخدم
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Notification>>> GetUserNotifications(string userId, int page = 1)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            
            // يمكن للمستخدم فقط رؤية إشعاراته الخاصة
            if (currentUser?.Id != userId)
            {
                return Forbid();
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(userId, page);
            return Ok(notifications);
        }

        /// <summary>
        /// الحصول على عدد الإشعارات غير المقروءة
        /// </summary>
        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var count = await _notificationService.GetUnreadCountAsync(currentUser.Id);
            return Ok(count);
        }

        /// <summary>
        /// تحديد الإشعار كمقروء
        /// </summary>
        [HttpPost("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            await _notificationService.MarkAsReadAsync(notificationId);
            return Ok();
        }

        /// <summary>
        /// تحديد جميع الإشعارات كمقروءة
        /// </summary>
        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            await _notificationService.MarkAllAsReadAsync(currentUser.Id);
            return Ok();
        }

        /// <summary>
        /// حذف إشعار
        /// </summary>
        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            await _notificationService.DeleteNotificationAsync(notificationId);
            return Ok();
        }
    }
}
