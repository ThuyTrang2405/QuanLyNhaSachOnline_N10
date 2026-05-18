using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaSachAPI.Models;

namespace QuanLyNhaSachAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "QuanTri")]
public class KhachHangController : ControllerBase
{
    private readonly QuanLyNhaSachContext _context;

    public KhachHangController(QuanLyNhaSachContext context)
    {
        _context = context;
    }

    // =============================================
    // GET /api/KhachHang — Danh sách khách hàng
    // =============================================
    [HttpGet]
    public async Task<IActionResult> GetDanhSach()
    {
        var danhSach = await _context.KhachHangs
            .Include(kh => kh.MaNdNavigation)
            .Select(kh => new
            {
                maNd        = kh.MaNd,
                hoTen       = kh.MaNdNavigation.HoTen,
                email       = kh.MaNdNavigation.Email,
                sdt         = kh.MaNdNavigation.Sdt ?? "",
                diaChi      = kh.MaNdNavigation.DiaChi ?? "",
                trangThaiTk = kh.TrangThaiTk ?? true
            })
            .OrderBy(kh => kh.hoTen)
            .ToListAsync();

        return Ok(danhSach);
    }

    // =============================================
    // PUT /api/KhachHang/{id}/trang-thai — Khóa/Mở tài khoản
    // =============================================
    [HttpPut("{id}/trang-thai")]
    public async Task<IActionResult> CapNhatTrangThai(int id, [FromBody] CapNhatTrangThaiRequest request)
    {
        var khachHang = await _context.KhachHangs.FindAsync(id);

        if (khachHang == null)
            return NotFound(new { message = "Không tìm thấy khách hàng" });

        khachHang.TrangThaiTk = request.TrangThai;
        await _context.SaveChangesAsync();

        string trangThaiText = request.TrangThai ? "mở khóa" : "khóa";
        return Ok(new { message = $"Đã {trangThaiText} tài khoản thành công" });
    }

    public class CapNhatTrangThaiRequest
    {
        public bool TrangThai { get; set; }
    }
}
