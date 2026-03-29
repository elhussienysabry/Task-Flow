using System;

namespace TaskManagementSystem.Models
{
    public class TaskAttachment
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty; // image, pdf, document
        public long FileSizeInBytes { get; set; }
        public string UploadedByUserId { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Task Task { get; set; } = null!;
        public virtual ApplicationUser UploadedByUser { get; set; } = null!;
    }
}
