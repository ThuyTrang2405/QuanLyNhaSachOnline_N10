using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Services;
using System.Security.Claims;

namespace QuanLyNhaSachAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GioHangController : ControllerBase
    {
        private readonly IGioHangService _gioHangService;

        public GioHangController(IGioHangService gioHangService)
        {
            _gioHangService = gioHangService;
        }

        private int GetMaNd()
        {
            var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? User.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
            return int.Parse(sub!);
        }

        [HttpGet]
        public async Task<IActionResult> GetGioHang()
        {
            var gioHang = await _gioHangService.LayGioHangAsync(GetMaNd());
            return Ok(gioHang);
        }

        [HttpPost]
        public async Task<IActionResult> ThemVaoGio([FromBody] ThemVaoGioRequestDTO request)
        {
            try
            {
                await _gioHangService.ThemVaoGioAsync(GetMaNd(), request);
                return Ok(new { message = "Đã thêm vào giỏ hàng" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{maSach}")]
        public async Task<IActionResult> CapNhatSoLuong(int maSach, [FromBody] CapNhatSoLuongRequestDTO request)
        {
            try
            {
                await _gioHangService.CapNhatSoLuongAsync(GetMaNd(), maSach, request.SoLuong);
                return Ok(new { message = "Đã cập nhật số lượng" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{maSach}")]
        public async Task<IActionResult> XoaKhoiGio(int maSach)
        {
            try
            {
                await _gioHangService.XoaKhoiGioAsync(GetMaNd(), maSach);
                return Ok(new { message = "Đã xóa khỏi giỏ hàng" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> XoaToanBoGio()
        {
            await _gioHangService.XoaToanBoGioAsync(GetMaNd());
            return Ok(new { message = "Đã xóa toàn bộ giỏ hàng" });
        }
    }
}