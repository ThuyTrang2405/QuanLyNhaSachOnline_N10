using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Services;

namespace QuanLyNhaSachAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SachController : ControllerBase
    {
        private readonly ISachService _sachService;

        public SachController(ISachService sachService)
        {
            _sachService = sachService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetSaches()
        {
            var danhSachSach = await _sachService.LayDanhSachSachAsync();
            return Ok(danhSachSach);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSach(int id)
        {
            try
            {
                var sach = await _sachService.LayChiTietSachAsync(id);
                return Ok(sach);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "QuanTri")] 
        public async Task<IActionResult> ThemSach([FromBody] SachRequestDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var sachMoi = await _sachService.ThemSachAsync(request);
            return Ok(new { message = "Thêm sách thành công!", data = sachMoi });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> CapNhatSach(int id, [FromBody] SachRequestDTO request)
        {
            try
            {
                await _sachService.CapNhatSachAsync(id, request);
                return Ok(new { message = "Cập nhật thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> XoaSach(int id)
        {
            try
            {
                await _sachService.XoaSachAsync(id);
                return Ok(new { message = "Đã chuyển sách sang trạng thái ngừng kinh doanh!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}