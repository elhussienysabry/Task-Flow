using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using TaskManagementSystem.Models;

namespace TaskManagementSystem.ViewModels
{
    public class TaskViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public Models.TaskStatus Status { get; set; }

        public TaskPriority Priority { get; set; }

        public DateTime? Deadline { get; set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;

        public string? AssignedUserId { get; set; }
        public string? AssignedUserName { get; set; }

        public DateTime CreatedAt { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<CommentViewModel> Comments { get; set; } = Enumerable.Empty<CommentViewModel>();
    }
}
