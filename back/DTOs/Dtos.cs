using System.ComponentModel.DataAnnotations;

namespace CoursePlatform.API.DTOs
{
    public record RegisterDto([Required] string Email, [Required] string Password, [Required] string FullName);
    public record LoginDto([Required] string Email, [Required] string Password);
    public record AuthResponseDto(string Token, string Email, string FullName);

    public record CourseDto(Guid Id, string Title, string Description, string Status, DateTime CreatedAt, DateTime UpdatedAt, int LessonCount);
    public record CreateCourseDto([Required] string Title, string Description);
    public record UpdateCourseDto([Required] string Title, string Description);
    public record CourseSummaryDto(string Title, int TotalLessons, DateTime LastModified);

    public record LessonDto(Guid Id, Guid CourseId, string Title, string Content, int Order, DateTime CreatedAt, DateTime UpdatedAt);
    public record CreateLessonDto([Required] Guid CourseId, [Required] string Title, string Content, int Order);
    public record UpdateLessonDto([Required] string Title, string Content, int Order);
}