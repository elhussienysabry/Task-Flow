using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        Task<IEnumerable<Project>> GetUserProjectsAsync(string userId);
    }

    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Project>> GetUserProjectsAsync(string userId)
        {
            return await _context.Projects
                .Where(p => p.OwnerId == userId)
                .ToListAsync();
        }
    }
}
