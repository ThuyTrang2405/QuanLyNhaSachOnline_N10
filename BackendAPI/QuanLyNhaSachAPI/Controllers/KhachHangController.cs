using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Services;

namespace QuanLyNhaSachAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "QuanTri")] 
    public class KhachHangController : ControllerBase
    {
        private readonly IKhachHangService _khachHangService;

        public KhachHangController(IKhachHangService khachHangService)
        {
            _khachHangService = khachHangService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSach()
        {
            var danhSach = await _khachHangService.LayDanhSachKhachHangAsync();
            return Ok(danhSach);
        }


        [HttpPut("{id}/trang-thai")]
        public async Task<IActionResult> CapNhatTrangThai(int id, [FromBody] CapNhatTrangThaiRequestDTO request)
        {
            try
            {
                string trangThaiText = await _khachHangService.CapNhatTrangThaiAsync(id, request.TrangThai);
                return Ok(new { message = $"Đã {trangThaiText} tài khoản thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}