using TaskManagementSystem.Models;

namespace TaskManagementSystem.Repositories
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
    }

    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(Data.ApplicationDbContext context) : base(context)
        {
        }
    }
}
