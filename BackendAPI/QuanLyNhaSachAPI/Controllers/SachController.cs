using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaSachAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace QuanLyNhaSachAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SachController : ControllerBase
    {
        private readonly QuanLyNhaSachContext _context;
        public SachController(QuanLyNhaSachContext context) => _context = context;

        // 1. Lấy danh sách sách (Dùng cho trang Home)
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetSaches()
        {
            var danhSachSach = await _context.Saches
                .Where(s => s.TrangThaiS == true)
                .Include(s => s.MaTgNavigation)
                .Include(s => s.MaTlNavigation)
                .Select(s => new
                {
                    // Trả về tên trường VIẾT HOA chữ cái đầu để khớp với Interface Sach trong Angular
                    MaSach      = s.MaSach,
                    TenSach     = s.TenSach,
                    GiaGoc      = s.GiaGoc,
                    HinhAnh     = s.HinhAnh ?? "",
                    MoTa        = s.MoTa ?? "",
                    SlTon       = s.Slton ?? 0,
                    // Lấy trực tiếp tên Tác giả và Thể loại từ bảng liên kết
                    TenTacGia   = s.MaTgNavigation != null ? s.MaTgNavigation.TenTg : "Đang cập nhật",
                    TenTheLoai  = s.MaTlNavigation != null ? s.MaTlNavigation.TenTl : "Đang cập nhật",
                    MaTG        = s.MaTg,
                    MaTL        = s.MaTl
                })
                .ToListAsync();

            return Ok(danhSachSach);
        }

        // 2. Lấy chi tiết một cuốn sách
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSach(int id)
        {
            var sach = await _context.Saches
                .Where(s => s.MaSach == id && s.TrangThaiS == true)
                .Include(s => s.MaTgNavigation)
                .Include(s => s.MaTlNavigation)
                .Select(s => new
                {
                    MaSach      = s.MaSach,
                    TenSach     = s.TenSach,
                    GiaGoc      = s.GiaGoc,
                    HinhAnh     = s.HinhAnh ?? "",
                    MoTa        = s.MoTa ?? "",
                    SlTon       = s.Slton ?? 0,
                    TenTacGia   = s.MaTgNavigation != null ? s.MaTgNavigation.TenTg : "Đang cập nhật",
                    TenTheLoai  = s.MaTlNavigation != null ? s.MaTlNavigation.TenTl : "Đang cập nhật",
                    MaTG        = s.MaTg,
                    MaTL        = s.MaTl
                })
                .FirstOrDefaultAsync();

            if (sach == null)
                return NotFound(new { message = "Không tìm thấy sách" });

            return Ok(sach);
        }

        // 3. Thêm sách mới
        [HttpPost]
        public async Task<IActionResult> ThemSach([FromBody] Sach sachMoi)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Saches.Add(sachMoi);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Thêm sách thành công!", data = sachMoi });
        }

        // 4. Cập nhật thông tin sách
        [HttpPut("{id}")]
        public async Task<IActionResult> CapNhatSach(int id, [FromBody] Sach sachCapNhat)
        {
            var sach = await _context.Saches.FirstOrDefaultAsync(s => s.MaSach == id);
            if (sach == null) return NotFound(new { message = "Không tìm thấy sách!" });

            sach.TenSach = sachCapNhat.TenSach;
            sach.GiaGoc  = sachCapNhat.GiaGoc;
            sach.MoTa    = sachCapNhat.MoTa;
            sach.Slton   = sachCapNhat.Slton;
            sach.HinhAnh = sachCapNhat.HinhAnh;
            sach.MaTl    = sachCapNhat.MaTl;
            sach.MaTg    = sachCapNhat.MaTg;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Cập nhật thành công!" });
        }

        // 5. Xóa sách (Xóa mềm - đổi trạng thái)
        [HttpDelete("{id}")]
        public async Task<IActionResult> XoaSach(int id)
        {
            var sach = await _context.Saches.FirstOrDefaultAsync(s => s.MaSach == id);
            if (sach == null) return NotFound(new { message = "Không tìm thấy sách!" });

            sach.TrangThaiS = false;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Đã chuyển sách sang trạng thái ngừng kinh doanh!" });
        }
    }
}