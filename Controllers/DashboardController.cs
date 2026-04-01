using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    public class DashboardController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public DashboardController(IProjectRepository projectRepository, ITaskRepository taskRepository, 
            IActivityRepository activityRepository, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _activityRepository = activityRepository;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Get all projects owned by the user
            var projects = await _projectRepository.GetAllAsync();
            var userProjects = projects.Where(p => p.OwnerId == userId).ToList();

            // Get all tasks (assigned to user OR in user's projects)
            var tasks = await _taskRepository.GetAllAsync();
            var userTasks = tasks.Where(t => 
                t.AssignedUserId == userId || 
                userProjects.Any(p => p.Id == t.ProjectId)
            ).ToList();

            var overdueTasks = userTasks.Where(t => 
                t.Deadline < System.DateTime.Now && 
                t.Status != Models.TaskStatus.Done
            ).Count();

            var tasksInProgress = userTasks.Where(t => t.Status == Models.TaskStatus.InProgress).Count();

            var currentMonth = System.DateTime.Now.Month;
            var currentYear = System.DateTime.Now.Year;
            var completedThisMonth = userTasks.Where(t => 
                t.Status == Models.TaskStatus.Done &&
                t.CreatedAt.Month == currentMonth &&
                t.CreatedAt.Year == currentYear
            ).Count();

            var avgCompletion = userTasks.Count > 0 
                ? (double)userTasks.Count(t => t.Status == Models.TaskStatus.Done) / userTasks.Count * 100 
                : 0;

            var activities = await _activityRepository.GetAllAsync();
            var userActivities = activities.Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToList();

            var viewModel = new DashboardViewModel
            {
                TotalProjects = userProjects.Count(),
                TotalTasks = userTasks.Count(),
                OverdueTasks = overdueTasks,
                TasksInProgress = tasksInProgress,
                CompletedTasksThisMonth = completedThisMonth,
                AvgCompletionRate = avgCompletion,
                RecentTasks = _mapper.Map<System.Collections.Generic.List<TaskViewModel>>(
                    userTasks.OrderByDescending(t => t.CreatedAt).Take(5)
                ),
                RecentActivities = _mapper.Map<System.Collections.Generic.List<ActivityViewModel>>(userActivities),
                TaskDistribution = userTasks.GroupBy(t => t.Status.ToString())
                    .ToDictionary(g => g.Key, g => g.Count()),
                MonthlyStats = new System.Collections.Generic.Dictionary<string, int>
                {
                    { "Completed", completedThisMonth },
                    { "InProgress", tasksInProgress },
                    { "Overdue", overdueTasks }
                }
            };

            return View(viewModel);
        }
    }
}
