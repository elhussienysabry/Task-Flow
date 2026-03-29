using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementSystem.Models
{
    /// <summary>

    /// </summary>
    public class TaskWatcher
    {
        public int Id { get; set; }

        [Required]
        public int TaskId { get; set; }

        [ForeignKey("TaskId")]
        public virtual Task? Task { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

       
        public System.DateTime AddedAt { get; set; } = System.DateTime.UtcNow;

     
        public WatcherType WatchType { get; set; } = WatcherType.Normal;
    }

    public enum WatcherType
    {
        Normal = 0,
        Assigned = 1,
        Reporter = 2,
        VotedFor = 3
    }
}
