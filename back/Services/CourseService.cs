using Microsoft.EntityFrameworkCore;
using CoursePlatform.API.Data;
using CoursePlatform.API.DTOs;
using CoursePlatform.API.Models;

namespace CoursePlatform.API.Services
{
    public class CourseService
    {
        private readonly AppDbContext _context;
        public CourseService(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<CourseDto>> GetAllAsync(string? status, int page, int pageSize)
        {
            var query = _context.Courses.Include(c => c.Lessons).AsQueryable();
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<CourseStatus>(status, true, out var st)) query = query.Where(c => c.Status == st);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return items.Select(c => new CourseDto(c.Id, c.Title, c.Description, c.Status.ToString(), c.CreatedAt, c.UpdatedAt, c.Lessons.Count(l => !l.IsDeleted)));
        }

        public async Task<CourseDto?> GetByIdAsync(Guid id)
        {
            var c = await _context.Courses.Include(x => x.Lessons).FirstOrDefaultAsync(x => x.Id == id);
            return c == null ? null : new CourseDto(c.Id, c.Title, c.Description, c.Status.ToString(), c.CreatedAt, c.UpdatedAt, c.Lessons.Count(l => !l.IsDeleted));
        }

        public async Task<CourseDto> CreateAsync(CreateCourseDto dto)
        {
            var course = new Course { Title = dto.Title, Description = dto.Description };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return new CourseDto(course.Id, course.Title, course.Description, course.Status.ToString(), course.CreatedAt, course.UpdatedAt, 0);
        }

        public async Task UpdateAsync(Guid id, UpdateCourseDto dto)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) throw new Exception("Not found");
            course.Title = dto.Title; course.Description = dto.Description;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCourseAsync(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null) { course.IsDeleted = true; await _context.SaveChangesAsync(); }
        }

        public async Task PublishCourseAsync(Guid id)
        {
            var course = await _context.Courses.Include(c => c.Lessons).FirstOrDefaultAsync(c => c.Id == id);
            if (course == null) throw new Exception("Not found");
            if (!course.Lessons.Any(l => !l.IsDeleted)) throw new InvalidOperationException("Cannot publish a course with no lessons.");
            course.Status = CourseStatus.Published;
            await _context.SaveChangesAsync();
        }

        public async Task UnpublishAsync(Guid id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null) { course.Status = CourseStatus.Draft; await _context.SaveChangesAsync(); }
        }

        public async Task<CourseSummaryDto?> GetSummaryAsync(Guid id)
        {
            var c = await _context.Courses.Include(x => x.Lessons).FirstOrDefaultAsync(x => x.Id == id);
            return c == null ? null : new CourseSummaryDto(c.Title, c.Lessons.Count(l => !l.IsDeleted), c.UpdatedAt);
        }
    }
}