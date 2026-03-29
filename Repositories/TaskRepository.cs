using Task = TaskManagementSystem.Models.Task;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories
{
    public interface ITaskRepository : IGenericRepository<Task>
    {
    }

    public class TaskRepository : GenericRepository<Task>, ITaskRepository
    {
        public TaskRepository(Data.ApplicationDbContext context) : base(context)
        {
        }
    }
}
