using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.Repositories
{
    public interface ITaskAttachmentRepository : IGenericRepository<TaskAttachment>
    {
        Task<List<TaskAttachment>> GetTaskAttachmentsAsync(int taskId);
        Task<TaskAttachment?> GetAttachmentAsync(int attachmentId);
    }

    public class TaskAttachmentRepository : GenericRepository<TaskAttachment>, ITaskAttachmentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TaskAttachmentRepository(ApplicationDbContext context) : base(context)
        {
            _dbContext = context;
        }

        public async Task<List<TaskAttachment>> GetTaskAttachmentsAsync(int taskId)
        {
            return await _dbContext.TaskAttachments
                .Where(ta => ta.TaskId == taskId)
                .OrderByDescending(ta => ta.UploadedAt)
                .ToListAsync();
        }

        public async Task<TaskAttachment?> GetAttachmentAsync(int attachmentId)
        {
            return await _dbContext.TaskAttachments
                .Include(ta => ta.UploadedByUser)
                .FirstOrDefaultAsync(ta => ta.Id == attachmentId);
        }
    }
}
