using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;
using Task = TaskManagementSystem.Models.Task;

namespace TaskManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<Task> Tasks { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Activity> Activities { get; set; } = null!;
        public DbSet<ProjectCollaborator> ProjectCollaborators { get; set; } = null!;
        public DbSet<TaskAttachment> TaskAttachments { get; set; } = null!;
        public DbSet<TaskAISuggestion> TaskAISuggestions { get; set; } = null!;
        public DbSet<SprintBoard> SprintBoards { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<TaskWatcher> TaskWatchers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Project>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Task>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Task>()
                .HasOne(t => t.AssignedUser)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.AssignedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Activity>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Activity>()
                .HasOne(a => a.Task)
                .WithMany()
                .HasForeignKey(a => a.TaskId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Activity>()
                .HasOne(a => a.Project)
                .WithMany()
                .HasForeignKey(a => a.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ProjectCollaborator>()
                .HasOne(pc => pc.Project)
                .WithMany(p => p.Collaborators)
                .HasForeignKey(pc => pc.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProjectCollaborator>()
                .HasOne(pc => pc.User)
                .WithMany()
                .HasForeignKey(pc => pc.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Task Attachment relationships
            builder.Entity<TaskAttachment>()
                .HasOne(ta => ta.Task)
                .WithMany(t => t.Attachments)
                .HasForeignKey(ta => ta.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TaskAttachment>()
                .HasOne(ta => ta.UploadedByUser)
                .WithMany()
                .HasForeignKey(ta => ta.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // AI Suggestion relationships
            builder.Entity<TaskAISuggestion>()
                .HasOne(tas => tas.Task)
                .WithMany(t => t.AISuggestions)
                .HasForeignKey(tas => tas.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sprint Board relationships
            builder.Entity<SprintBoard>()
                .HasOne(sb => sb.Project)
                .WithMany()
                .HasForeignKey(sb => sb.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Task>()
                .HasOne(t => t.ParentTask)
                .WithMany(t => t.SubTasks)
                .HasForeignKey(t => t.ParentTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Task>()
                .HasOne(t => t.SprintBoard)
                .WithMany(sb => sb.Tasks)
                .HasForeignKey(t => t.SprintBoardId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany()
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification relationships
            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Notification>()
                .HasOne(n => n.RelatedTask)
                .WithMany()
                .HasForeignKey(n => n.RelatedTaskId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Notification>()
                .HasOne(n => n.RelatedProject)
                .WithMany()
                .HasForeignKey(n => n.RelatedProjectId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Notification>()
                .HasOne(n => n.TriggeredByUser)
                .WithMany()
                .HasForeignKey(n => n.TriggeredByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Create index for faster notification queries
            builder.Entity<Notification>()
                .HasIndex(n => new { n.UserId, n.IsRead, n.CreatedAt });

            // TaskWatcher relationships
            builder.Entity<TaskWatcher>()
                .HasOne(tw => tw.Task)
                .WithMany(t => t.Watchers)
                .HasForeignKey(tw => tw.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TaskWatcher>()
                .HasOne(tw => tw.User)
                .WithMany()
                .HasForeignKey(tw => tw.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index for finding watchers efficiently
            builder.Entity<TaskWatcher>()
                .HasIndex(tw => new { tw.TaskId, tw.UserId })
                .IsUnique();
        }
    }
}
