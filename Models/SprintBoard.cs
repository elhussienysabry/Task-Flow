using System;
using System.Collections.Generic;

namespace TaskManagementSystem.Models
{
    public class SprintBoard
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Goal { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = "Planning"; // Planning, Active, Completed
        public int VelocityTarget { get; set; } = 0;
        public int ActualVelocity { get; set; } = 0;

        // Navigation properties
        public virtual Project? Project { get; set; }
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
