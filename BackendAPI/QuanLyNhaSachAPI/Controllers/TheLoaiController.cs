using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Services;

namespace QuanLyNhaSachAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheLoaiController : ControllerBase
    {
        private readonly ITheLoaiService _theLoaiService;

        public TheLoaiController(ITheLoaiService theLoaiService)
        {
            _theLoaiService = theLoaiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDanhSach()
        {
            var list = await _theLoaiService.LayDanhSachTheLoaiAsync();
            return Ok(list);
        }

        [HttpPost]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> ThemTheLoai([FromBody] TheLoaiRequestDTO req)
        {
            try
            {
                var theLoaiMoi = await _theLoaiService.ThemTheLoaiAsync(req);
                return Ok(new { message = "Thêm thể loại thành công!", maTl = theLoaiMoi.MaTl, tenTl = theLoaiMoi.TenTl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> SuaTheLoai(int id, [FromBody] TheLoaiRequestDTO req)
        {
            try
            {
                await _theLoaiService.CapNhatTheLoaiAsync(id, req);
                return Ok(new { message = "Cập nhật thành công!" });
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
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> XoaTheLoai(int id)
        {
            try
            {
                await _theLoaiService.XoaTheLoaiAsync(id);
                return Ok(new { message = "Đã xóa thể loại" });
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