using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories
{
    public interface IActivityRepository : IGenericRepository<Activity>
    {
    }

    public class ActivityRepository : GenericRepository<Activity>, IActivityRepository
    {
        public ActivityRepository(Data.ApplicationDbContext context) : base(context)
        {
        }
    }
}
