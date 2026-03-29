using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagementSystem.Models;
using TaskManagementSystem.Repositories;
using TaskManagementSystem.ViewModels;
using Task = TaskManagementSystem.Models.Task;

namespace TaskManagementSystem.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly Services.INotificationService _notificationService;
        private readonly Repositories.ITaskWatcherRepository _watcherRepository;

        public TasksController(ITaskRepository taskRepository, IProjectRepository projectRepository, ICommentRepository commentRepository, 
            IActivityRepository activityRepository, UserManager<ApplicationUser> userManager, IMapper mapper,
            Services.INotificationService notificationService, Repositories.ITaskWatcherRepository watcherRepository)
        {
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
            _commentRepository = commentRepository;
            _activityRepository = activityRepository;
            _userManager = userManager;
            _mapper = mapper;
            _notificationService = notificationService;
            _watcherRepository = watcherRepository;
        }

        public async Task<IActionResult> Index(int projectId, string searchString, Models.TaskStatus? status, TaskPriority? priority)
        {
            // إذا لم يتم تحديد مشروع، طلب من المستخدم الاختيار
            if (projectId == 0)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                var userProjects = (await _projectRepository.GetUserProjectsAsync(userId)).ToList();
                
                if (!userProjects.Any())
                    return RedirectToAction("Create", "Projects");
                
                // إذا كان لديه مشروع واحد فقط، اختره تلقائياً
                if (userProjects.Count() == 1)
                {
                    projectId = userProjects.First().Id;
                }
                else
                {
                    // إعادة توجيه إلى صفحة اختيار المشروع
                    return RedirectToAction("SelectProject");
                }
            }

            var tasks = await _taskRepository.GetAllAsync();
            tasks = tasks.Where(t => t.ProjectId == projectId);

            if (!string.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(t => t.Title.Contains(searchString, System.StringComparison.OrdinalIgnoreCase));
            }

            if (status.HasValue)
            {
                tasks = tasks.Where(t => t.Status == status.Value);
            }

            if (priority.HasValue)
            {
                tasks = tasks.Where(t => t.Priority == priority.Value);
            }

            ViewBag.Project = await _projectRepository.GetByIdAsync(projectId);
            var taskViewModels = _mapper.Map<System.Collections.Generic.IEnumerable<TaskViewModel>>(tasks);
            return View(taskViewModels);
        }

        public async Task<IActionResult> SelectProject()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var projects = await _projectRepository.GetUserProjectsAsync(userId);
            return View(projects);
        }

        public async Task<IActionResult> Details(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var taskViewModel = _mapper.Map<TaskViewModel>(task);
            var comments = await _commentRepository.GetAllAsync();
            taskViewModel.Comments = _mapper.Map<System.Collections.Generic.IEnumerable<CommentViewModel>>(comments.Where(c => c.TaskId == id));
            return View(taskViewModel);
        }

        public async Task<IActionResult> Create(int projectId)
        {
            // احصل على المستخدمين العاديين فقط (ليس Admin)
            var users = await _userManager.Users.ToListAsync();
            var nonAdminUsers = new System.Collections.Generic.List<ApplicationUser>();
            
            foreach (var user in users)
            {
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    nonAdminUsers.Add(user);
                }
            }

            var taskViewModel = new TaskViewModel
            {
                ProjectId = projectId,
                Users = new SelectList(nonAdminUsers, "Id", "FullName")
            };
            return View(taskViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaskViewModel model)
        {
            if (ModelState.IsValid)
            {
                var task = _mapper.Map<Task>(model);
                task.CreatedAt = System.DateTime.Now;
                await _taskRepository.AddAsync(task);
                await _taskRepository.SaveChangesAsync();
                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                var activity = new Activity
                {
                    ActionType = "Created",
                    Description = $"تم إنشاء مهمة جديدة: {task.Title}",
                    TaskId = task.Id,
                    ProjectId = task.ProjectId,
                    UserId = userId,
                    CreatedAt = System.DateTime.Now
                };
                await _activityRepository.AddAsync(activity);
                await _activityRepository.SaveChangesAsync();

                // إذا تم تعيين للمستخدم، أرسل إشعار
                if (!string.IsNullOrEmpty(task.AssignedUserId))
                {
                    await _notificationService.NotifyTaskAssignmentAsync(task, task.AssignedUserId, userId);
                    await _watcherRepository.AddWatcherAsync(task.Id, task.AssignedUserId, WatcherType.Assigned);
                }

                // أضف منشئ المهمة كمتابع
                await _watcherRepository.AddWatcherAsync(task.Id, userId, WatcherType.Reporter);
                
                return RedirectToAction(nameof(Index), new { projectId = model.ProjectId });
            }
            var users = await _userManager.Users.ToListAsync();
            model.Users = new SelectList(users, "Id", "UserName", model.AssignedUserId);
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var users = await _userManager.Users.ToListAsync();
            var taskViewModel = _mapper.Map<TaskViewModel>(task);
            taskViewModel.Users = new SelectList(users, "Id", "UserName", task.AssignedUserId);
            return View(taskViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaskViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                var oldAssignedUserId = task.AssignedUserId;
                var oldStatus = task.Status;
                var oldPriority = task.Priority;
                
                _mapper.Map(model, task);
                _taskRepository.Update(task);
                await _taskRepository.SaveChangesAsync();
                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                var activity = new Activity
                {
                    ActionType = "Updated",
                    Description = $"تم تحديث المهمة: {task.Title}",
                    TaskId = task.Id,
                    ProjectId = task.ProjectId,
                    UserId = userId,
                    CreatedAt = System.DateTime.Now
                };
                await _activityRepository.AddAsync(activity);
                await _activityRepository.SaveChangesAsync();

                // إرسال إشعارات التغييرات
                if (oldAssignedUserId != task.AssignedUserId)
                {
                    if (!string.IsNullOrEmpty(task.AssignedUserId))
                    {
                        await _notificationService.NotifyTaskAssignmentAsync(task, task.AssignedUserId, userId);
                        await _watcherRepository.AddWatcherAsync(task.Id, task.AssignedUserId, WatcherType.Assigned);
                    }
                }

                if (oldStatus != task.Status)
                {
                    if (task.Status == Models.TaskStatus.Done)
                    {
                        await _notificationService.NotifyTaskCompletionAsync(task, userId);
                    }
                    else if (!string.IsNullOrEmpty(task.AssignedUserId) && task.AssignedUserId != userId)
                    {
                        await _notificationService.NotifyTaskUpdateAsync(task, userId);
                    }
                }

                if (oldPriority != task.Priority && !string.IsNullOrEmpty(task.AssignedUserId))
                {
                    await _notificationService.CreateNotificationAsync(
                        task.AssignedUserId,
                        $"تم تغيير أولوية المهمة: {task.Title}",
                        NotificationType.TaskPriorityChanged,
                        $"الأولوية الجديدة: {task.Priority}",
                        task.Id,
                        task.ProjectId,
                        $"/Tasks/Details/{task.Id}",
                        userId
                    );
                }
                
                return RedirectToAction(nameof(Index), new { projectId = model.ProjectId });
            }
            var users = await _userManager.Users.ToListAsync();
            model.Users = new SelectList(users, "Id", "UserName", model.AssignedUserId);
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            var projectId = task.ProjectId;
            var taskTitle = task.Title;
            
            _taskRepository.Remove(task);
            await _taskRepository.SaveChangesAsync();
            
            // تسجيل النشاط
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var activity = new Activity
            {
                ActionType = "Deleted",
                Description = $"تم حذف المهمة: {taskTitle}",
                TaskId = null, // المهمة تم حذفها
                ProjectId = projectId,
                UserId = userId,
                CreatedAt = System.DateTime.Now
            };
            await _activityRepository.AddAsync(activity);
            await _activityRepository.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index), new { projectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int taskId, string content)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var comment = new Comment
            {
                Content = content,
                CreatedAt = System.DateTime.Now,
                TaskId = taskId,
                UserId = userId
            };

            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();
            
            var activity = new Activity
            {
                ActionType = "Commented",
                Description = $"تم إضافة تعليق على المهمة: {task.Title}",
                TaskId = taskId,
                ProjectId = task.ProjectId,
                UserId = userId,
                CreatedAt = System.DateTime.Now
            };
            await _activityRepository.AddAsync(activity);
            await _activityRepository.SaveChangesAsync();

            // إشعار بالتعليق الجديد
            if (!string.IsNullOrEmpty(task.AssignedUserId) && task.AssignedUserId != userId)
            {
                await _notificationService.CreateNotificationAsync(
                    task.AssignedUserId,
                    $"تم إضافة تعليق على المهمة: {task.Title}",
                    NotificationType.CommentAdded,
                    content,
                    task.Id,
                    task.ProjectId,
                    $"/Tasks/Details/{task.Id}",
                    userId
                );
            }

            return RedirectToAction(nameof(Details), new { id = taskId });
        }
    }
}
