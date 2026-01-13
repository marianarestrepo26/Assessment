using Microsoft.AspNetCore.Mvc;
using CoursePlatform.API.DTOs;
using CoursePlatform.API.Services;

namespace CoursePlatform.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try { return Ok(await _authService.RegisterAsync(dto)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try { return Ok(await _authService.LoginAsync(dto)); }
            catch (Exception ex) { return Unauthorized(ex.Message); }
        }
    }
}
