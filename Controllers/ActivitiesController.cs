using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagementSystem.Repositories;
using TaskManagementSystem.ViewModels;

namespace TaskManagementSystem.Controllers
{
    [Authorize]
    public class ActivitiesController : Controller
    {
        private readonly IActivityRepository _activityRepository;
        private readonly IMapper _mapper;

        public ActivitiesController(IActivityRepository activityRepository, IMapper mapper)
        {
            _activityRepository = activityRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var activities = await _activityRepository.GetAllAsync();
            
           
            var userActivities = activities.Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

            var activityViewModels = _mapper.Map<System.Collections.Generic.List<ActivityViewModel>>(userActivities);
            
            return View(activityViewModels);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearLog()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var activities = await _activityRepository.GetAllAsync();
            var userActivities = activities.Where(a => a.UserId == userId).ToList();

            foreach (var activity in userActivities)
            {
                _activityRepository.Remove(activity);
            }

            await _activityRepository.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
