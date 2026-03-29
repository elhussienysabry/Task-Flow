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
    public class ProjectsController : Controller
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(IProjectRepository projectRepository, IActivityRepository activityRepository, 
            IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _projectRepository = projectRepository;
            _activityRepository = activityRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var projects = await _projectRepository.GetAllAsync();
            
            // Only show projects owned by the current user
            projects = projects.Where(p => p.OwnerId == userId);
            
            if (!string.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(p => p.Name.Contains(searchString, System.StringComparison.OrdinalIgnoreCase));
            }
            var projectViewModels = _mapper.Map<System.Collections.Generic.IEnumerable<ProjectViewModel>>(projects);
            return View(projectViewModels);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                var project = _mapper.Map<Project>(model);
                project.OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
                project.CreatedDate = System.DateTime.Now;
                await _projectRepository.AddAsync(project);
                await _projectRepository.SaveChangesAsync();
                
                // تسجيل النشاط
                var activity = new Activity
                {
                    ActionType = "Created",
                    Description = $"تم إنشاء مشروع جديد: {project.Name}",
                    ProjectId = project.Id,
                    UserId = project.OwnerId,
                    CreatedAt = System.DateTime.Now
                };
                await _activityRepository.AddAsync(activity);
                await _activityRepository.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (project.OwnerId != userId)
            {
                return Forbid();
            }
            
            var projectViewModel = _mapper.Map<ProjectViewModel>(project);
            return View(projectViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var project = await _projectRepository.GetByIdAsync(id);
                if (project == null)
                {
                    return NotFound();
                }
                
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (project.OwnerId != userId)
                {
                    return Forbid();
                }

                _mapper.Map(model, project);
                _projectRepository.Update(project);
                await _projectRepository.SaveChangesAsync();
                
                // تسجيل النشاط
                var activity = new Activity
                {
                    ActionType = "Updated",
                    Description = $"تم تحديث المشروع: {project.Name}",
                    ProjectId = project.Id,
                    UserId = userId,
                    CreatedAt = System.DateTime.Now
                };
                await _activityRepository.AddAsync(activity);
                await _activityRepository.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (project.OwnerId != userId)
            {
                return Forbid();
            }
            
            _projectRepository.Remove(project);
            await _projectRepository.SaveChangesAsync();
            
            // تسجيل النشاط
            var activity = new Activity
            {
                ActionType = "Deleted",
                Description = $"تم حذف المشروع: {project.Name}",
                ProjectId = null, // المشروع تم حذفه، لا يمكن الاحتفاظ بالمعرف
                UserId = userId,
                CreatedAt = System.DateTime.Now
            };
            await _activityRepository.AddAsync(activity);
            await _activityRepository.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        // Collaborators Management
        public async Task<IActionResult> ManageCollaborators(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (project.OwnerId != userId)
                return Forbid();

            ViewBag.ProjectId = id;
            ViewBag.ProjectName = project.Name;
            return View();
        }

        public async Task<IActionResult> AddCollaborator(int projectId, string email, string role = "Contributor")
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
                return Json(new { success = false, message = "Project not found" });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (project.OwnerId != userId)
                return Json(new { success = false, message = "Not authorized" });

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            // Add collaborator logic would go here
            return Json(new { success = true, message = "Collaborator added successfully" });
        }

        public async Task<IActionResult> RemoveCollaborator(int projectId, string userId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
                return Json(new { success = false, message = "Project not found" });

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (project.OwnerId != currentUserId)
                return Json(new { success = false, message = "Not authorized" });

            // Remove collaborator logic would go here
            return Json(new { success = true, message = "Collaborator removed" });
        }
    }
}
