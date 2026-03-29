using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystem.Models
{
    public class ProjectCollaborator
    {
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; } = null!;

        [Required]
        public string UserId { get; set; } = string.Empty;
        public virtual ApplicationUser User { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Contributor"; // Editor, Viewer, Contributor

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string? Note { get; set; }
    }
}
