using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Services;

namespace QuanLyNhaSachAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DangKyDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _authService.DangKyAsync(dto);
                return Ok(new { message = "Đăng ký thành công! Vui lòng đăng nhập." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DangNhapDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _authService.DangNhapAsync(dto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("hash-all-passwords")]
        [AllowAnonymous]
        public async Task<IActionResult> HashAllPasswords()
        {
            int count = await _authService.HashAllPasswordsAsync();
            return Ok(new { message = $"Hoàn tất! Đã hash {count} tài khoản. Các tài khoản đã hash trước đó được giữ nguyên." });
        }
    }
}