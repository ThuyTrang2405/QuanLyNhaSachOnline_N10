using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Services;
using System.Security.Claims;

namespace QuanLyNhaSachAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DonHangController : ControllerBase
    {
        private readonly IDonHangService _donHangService;

        public DonHangController(IDonHangService donHangService)
        {
            _donHangService = donHangService;
        }


        [HttpPost("tao-don")]
        public async Task<IActionResult> TaoDonHang([FromBody] DonHangRequestDTO request)
        {
            string? maNdChuoi = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNdChuoi))
                return Unauthorized(new { message = "Không xác định được danh tính người dùng." });

            try
            {
                int maNd = int.Parse(maNdChuoi);
                await _donHangService.TaoDonHangAsync(maNd, request);
                return Ok(new { message = "Đặt hàng thành công!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (SqlException ex)
            {
                if (ex.Number == 50001)
                    return BadRequest(new { message = ex.Message });

                return StatusCode(500, new { message = "Hệ thống đang quá tải. Vui lòng thử lại sau giây lát!" });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Hệ thống đang quá tải. Vui lòng thử lại sau giây lát!" });
            }
        }


        [HttpGet]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> GetAllDonHang()
        {
            var donHangs = await _donHangService.LayTatCaDonHangAsync();
            return Ok(donHangs);
        }


        [HttpGet("cua-toi")]
        public async Task<IActionResult> GetDonHangCuaToi()
        {
            string? maNdChuoi = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(maNdChuoi))
                return Unauthorized(new { message = "Không xác định được danh tính người dùng." });

            try
            {
                int maNd = int.Parse(maNdChuoi);
                var donHangs = await _donHangService.LayDonHangCuaKhachAsync(maNd);
                return Ok(donHangs);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChiTietDonHang(int id)
        {
            try
            {
                var isUserAdmin = User.IsInRole("QuanTri");
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
                int maNd = int.Parse(userIdClaim!.Value);

                var result = await _donHangService.LayChiTietDonHangAsync(id, maNd, isUserAdmin);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

       
        [HttpPut("{id}/trang-thai")]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> UpdateTrangThai(int id, [FromBody] CapNhatTrangThaiDHRequestDTO request)
        {
            try
            {
                string? adminIdChuoi = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int adminId = string.IsNullOrEmpty(adminIdChuoi) ? 0 : int.Parse(adminIdChuoi);

                var trangThaiMoi = await _donHangService.CapNhatTrangThaiAsync(id, request, adminId);
                return Ok(new { message = "Cập nhật trạng thái đơn hàng thành công!", trangThaiMoi = trangThaiMoi });
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
    }
}