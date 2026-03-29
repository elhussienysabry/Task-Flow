using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Data;
using TaskManagementSystem.Models;
using Task = TaskManagementSystem.Models.Task;

namespace TaskManagementSystem.Repositories
{
    public interface ITaskWatcherRepository
    {
        System.Threading.Tasks.Task AddWatcherAsync(int taskId, string userId, WatcherType watchType = WatcherType.Normal);
        System.Threading.Tasks.Task RemoveWatcherAsync(int taskId, string userId);
        System.Threading.Tasks.Task<bool> IsWatchingAsync(int taskId, string userId);
        System.Threading.Tasks.Task<List<TaskWatcher>> GetTaskWatchersAsync(int taskId);
        System.Threading.Tasks.Task<List<Task>> GetWatchedTasksAsync(string userId);
    }

    public class TaskWatcherRepository : ITaskWatcherRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskWatcherRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async System.Threading.Tasks.Task AddWatcherAsync(int taskId, string userId, WatcherType watchType = WatcherType.Normal)
        {
            // تحقق من عدم وجود المتابع بالفعل
            var existingWatcher = await _context.TaskWatchers
                .FirstOrDefaultAsync(w => w.TaskId == taskId && w.UserId == userId);

            if (existingWatcher == null)
            {
                var watcher = new TaskWatcher
                {
                    TaskId = taskId,
                    UserId = userId,
                    WatchType = watchType,
                    AddedAt = DateTime.UtcNow
                };

                _context.TaskWatchers.Add(watcher);
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task RemoveWatcherAsync(int taskId, string userId)
        {
            var watcher = await _context.TaskWatchers
                .FirstOrDefaultAsync(w => w.TaskId == taskId && w.UserId == userId);

            if (watcher != null)
            {
                _context.TaskWatchers.Remove(watcher);
                await _context.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task<bool> IsWatchingAsync(int taskId, string userId)
        {
            return await _context.TaskWatchers
                .AnyAsync(w => w.TaskId == taskId && w.UserId == userId);
        }

        public async System.Threading.Tasks.Task<List<TaskWatcher>> GetTaskWatchersAsync(int taskId)
        {
            return await _context.TaskWatchers
                .Include(w => w.User)
                .Where(w => w.TaskId == taskId)
                .ToListAsync();
        }

        public async System.Threading.Tasks.Task<List<Task>> GetWatchedTasksAsync(string userId)
        {
            return await _context.TaskWatchers
                .Where(w => w.UserId == userId)
                .Include(w => w.Task)
                .Select(w => w.Task!)
                .Where(t => t != null)
                .ToListAsync();
        }
    }
}
