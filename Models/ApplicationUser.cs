using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TaskManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        

        public string DisplayName { get; set; } = string.Empty;
        
        public string ProfileImageUrl { get; set; } = "/images/default-avatar.svg";
        public string Department { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        

        public DateTime? LastLoginDate { get; set; }
        
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        
  
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
