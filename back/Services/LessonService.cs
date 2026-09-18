using Microsoft.EntityFrameworkCore;
using CoursePlatform.API.Data;
using CoursePlatform.API.DTOs;
using CoursePlatform.API.Models;

namespace CoursePlatform.API.Services
{
    public class LessonService
    {
        private readonly AppDbContext _context;
        public LessonService(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<LessonDto>> GetByCourseIdAsync(Guid courseId)
        {
            var lessons = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.Order)
                .ToListAsync();
            return lessons.Select(l => new LessonDto(l.Id, l.CourseId, l.Title, l.Content, l.Order, l.CreatedAt, l.UpdatedAt));
        }

        public async Task<LessonDto> CreateAsync(CreateLessonDto dto)
        {
            var conflict = await _context.Lessons.AnyAsync(l => l.CourseId == dto.CourseId && l.Order == dto.Order && !l.IsDeleted);
            if (conflict) throw new InvalidOperationException("Duplicate lesson order.");

            var lesson = new Lesson { CourseId = dto.CourseId, Title = dto.Title, Content = dto.Content, Order = dto.Order };
            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();
            return new LessonDto(lesson.Id, lesson.CourseId, lesson.Title, lesson.Content, lesson.Order, lesson.CreatedAt, lesson.UpdatedAt);
        }

        public async Task UpdateAsync(Guid id, UpdateLessonDto dto)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) throw new Exception("Not found");

            var siblings = await _context.Lessons
                .Where(l => l.CourseId == lesson.CourseId && l.Id != lesson.Id && !l.IsDeleted)
                .ToListAsync();

            var oldOrder = lesson.Order;
            var newOrder = Math.Clamp(dto.Order, 1, siblings.Count + 1);

            if (newOrder > oldOrder)
            {
                foreach (var sibling in siblings.Where(l => l.Order > oldOrder && l.Order <= newOrder))
                    sibling.Order--;
            }
            else if (newOrder < oldOrder)
            {
                foreach (var sibling in siblings.Where(l => l.Order >= newOrder && l.Order < oldOrder))
                    sibling.Order++;
            }

            lesson.Title = dto.Title;
            lesson.Content = dto.Content;
            lesson.Order = newOrder;
            lesson.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson != null) { lesson.IsDeleted = true; await _context.SaveChangesAsync(); }
        }
    }
}