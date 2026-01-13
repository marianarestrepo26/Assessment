using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoursePlatform.API.DTOs;
using CoursePlatform.API.Services;

namespace CoursePlatform.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/lessons")]
    public class LessonsController : ControllerBase
    {
        private readonly LessonService _lessonService;

        public LessonsController(LessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet("course/{courseId}")]
        public async Task<IActionResult> GetByCourse(Guid courseId)
        {
            return Ok(await _lessonService.GetByCourseIdAsync(courseId));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateLessonDto dto)
        {
            try
            {
                return Ok(await _lessonService.CreateAsync(dto));
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateLessonDto dto)
        {
            try
            {
                await _lessonService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _lessonService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception) { return NotFound(); }
        }
    }
}
