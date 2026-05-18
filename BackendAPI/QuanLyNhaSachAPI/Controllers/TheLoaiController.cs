using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaSachAPI.Models;

namespace QuanLyNhaSachAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheLoaiController : ControllerBase
    {
        private readonly QuanLyNhaSachContext _context;
        public TheLoaiController(QuanLyNhaSachContext context) => _context = context;

        // GET /api/TheLoai — Lấy danh sách (public, dùng cho filter trang chủ)
        [HttpGet]
        public async Task<IActionResult> GetDanhSach()
        {
            var list = await _context.TheLoais
                .OrderBy(t => t.TenTl)
                .Select(t => new { maTl = t.MaTl, tenTl = t.TenTl })
                .ToListAsync();
            return Ok(list);
        }

        // POST /api/TheLoai — Thêm thể loại (chỉ Admin)
        [HttpPost]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> ThemTheLoai([FromBody] TheLoaiRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.TenTl))
                return BadRequest(new { message = "Tên thể loại không được để trống" });

            bool trung = await _context.TheLoais.AnyAsync(t => t.TenTl == req.TenTl.Trim());
            if (trung)
                return Conflict(new { message = "Thể loại này đã tồn tại" });

            var theLoai = new TheLoai { TenTl = req.TenTl.Trim() };
            _context.TheLoais.Add(theLoai);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Thêm thể loại thành công!", maTl = theLoai.MaTl, tenTl = theLoai.TenTl });
        }

        // PUT /api/TheLoai/{id} — Sửa tên thể loại (chỉ Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> SuaTheLoai(int id, [FromBody] TheLoaiRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.TenTl))
                return BadRequest(new { message = "Tên thể loại không được để trống" });

            var theLoai = await _context.TheLoais.FindAsync(id);
            if (theLoai == null)
                return NotFound(new { message = "Không tìm thấy thể loại" });

            bool trung = await _context.TheLoais.AnyAsync(t => t.TenTl == req.TenTl.Trim() && t.MaTl != id);
            if (trung)
                return Conflict(new { message = "Tên thể loại này đã tồn tại" });

            theLoai.TenTl = req.TenTl.Trim();
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thành công!" });
        }

        // DELETE /api/TheLoai/{id} — Xóa thể loại (chỉ Admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> XoaTheLoai(int id)
        {
            var theLoai = await _context.TheLoais.FindAsync(id);
            if (theLoai == null)
                return NotFound(new { message = "Không tìm thấy thể loại" });

            bool dangDung = await _context.Saches.AnyAsync(s => s.MaTl == id);
            if (dangDung)
                return BadRequest(new { message = "Không thể xóa vì đang có sách thuộc thể loại này" });

            _context.TheLoais.Remove(theLoai);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã xóa thể loại" });
        }
    }

    public class TheLoaiRequest
    {
        public string TenTl { get; set; } = "";
    }
}
