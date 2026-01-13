using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace CoursePlatform.API.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class User : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
    }

    public enum CourseStatus
    {
        Draft,
        Published
    }

    public class Course : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }

    public class Lesson : BaseEntity
    {
        public Guid CourseId { get; set; }
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Order { get; set; }
        public Course? Course { get; set; }
    }
}
