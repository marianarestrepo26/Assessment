using Xunit;
using Microsoft.EntityFrameworkCore;
using CoursePlatform.API.Data;
using CoursePlatform.API.Services;
using CoursePlatform.API.Models;
using CoursePlatform.API.DTOs;

namespace CoursePlatform.Tests
{
    public class BusinessRuleTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task PublishCourse_WithLessons_ShouldSucceed()
        {
            var context = GetInMemoryDbContext();
            var service = new CourseService(context);
            var course = new Course { Title = "Test", Status = CourseStatus.Draft };
            context.Courses.Add(course);
            context.Lessons.Add(new Lesson { CourseId = course.Id, Title = "L1", Order = 1 });
            await context.SaveChangesAsync();

            await service.PublishCourseAsync(course.Id);
            var updated = await context.Courses.FindAsync(course.Id);
            Assert.Equal(CourseStatus.Published, updated.Status);
        }

        [Fact]
        public async Task PublishCourse_WithoutLessons_ShouldFail()
        {
            var context = GetInMemoryDbContext();
            var service = new CourseService(context);
            var course = new Course { Title = "Empty", Status = CourseStatus.Draft };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.PublishCourseAsync(course.Id));
        }

        [Fact]
        public async Task CreateLesson_WithUniqueOrder_ShouldSucceed()
        {
            var context = GetInMemoryDbContext();
            var service = new LessonService(context);
            var courseId = Guid.NewGuid();
            var dto = new CreateLessonDto(courseId, "Title", "Content", 1);

            var result = await service.CreateAsync(dto);
            Assert.Equal(1, result.Order);
        }

        [Fact]
        public async Task CreateLesson_WithDuplicateOrder_ShouldFail()
        {
            var context = GetInMemoryDbContext();
            var service = new LessonService(context);
            var courseId = Guid.NewGuid();
            await service.CreateAsync(new CreateLessonDto(courseId, "L1", "C", 1));

            var duplicateDto = new CreateLessonDto(courseId, "L2", "C", 1);
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(duplicateDto));
        }

        [Fact]
        public async Task DeleteCourse_ShouldBeSoftDelete()
        {
            var context = GetInMemoryDbContext();
            var service = new CourseService(context);
            var course = new Course { Title = "DeleteMe" };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            await service.DeleteCourseAsync(course.Id);

            var deleted = await context.Courses.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == course.Id);
            Assert.True(deleted.IsDeleted);
        }
    }
}