using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using AsyncTask = System.Threading.Tasks.Task;

namespace TaskManagementSystem.Repositories
{
    public interface IProjectCollaboratorRepository : IGenericRepository<ProjectCollaborator>
    {
        System.Threading.Tasks.Task<List<ProjectCollaborator>> GetProjectCollaboratorsAsync(int projectId);

        System.Threading.Tasks.Task<ProjectCollaborator?> GetCollaboratorAsync(int projectId, string userId);

        System.Threading.Tasks.Task<bool> IsUserCollaboratorAsync(int projectId, string userId);

        System.Threading.Tasks.Task RemoveCollaboratorAsync(int projectId, string userId);
    }

    public class ProjectCollaboratorRepository 
        : GenericRepository<ProjectCollaborator>, IProjectCollaboratorRepository
    {
        public ProjectCollaboratorRepository(ApplicationDbContext context) 
            : base(context)
        {
        }

        public async System.Threading.Tasks.Task<List<ProjectCollaborator>> GetProjectCollaboratorsAsync(int projectId)
        {
            return await _context.ProjectCollaborators
                .Where(pc => pc.ProjectId == projectId)
                .Include(pc => pc.User)
                .OrderByDescending(pc => pc.AddedDate)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<ProjectCollaborator?> GetCollaboratorAsync(int projectId, string userId)
        {
            return await _context.ProjectCollaborators
                .FirstOrDefaultAsync(pc => pc.ProjectId == projectId && pc.UserId == userId);
        }

        public async System.Threading.Tasks.Task<bool> IsUserCollaboratorAsync(int projectId, string userId)
        {
            return await _context.ProjectCollaborators
                .AnyAsync(pc => pc.ProjectId == projectId && pc.UserId == userId);
        }

        public async System.Threading.Tasks.Task RemoveCollaboratorAsync(int projectId, string userId)
        {
            var collaborator = await GetCollaboratorAsync(projectId, userId);

            if (collaborator != null)
            {
                _context.ProjectCollaborators.Remove(collaborator);
                await _context.SaveChangesAsync();
            }
        }
    }
}