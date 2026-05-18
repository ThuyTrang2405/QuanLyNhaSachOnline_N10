using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaSachAPI.Models;
using System.Security.Claims;

namespace QuanLyNhaSachAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize] // phải đăng nhập mới dùng được giỏ hàng
public class GioHangController : ControllerBase
{
    private readonly QuanLyNhaSachContext _context;

    public GioHangController(QuanLyNhaSachContext context)
    {
        _context = context;
    }

    // Lấy MaND từ JWT token
    private int GetMaNd()
    {
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier)
               ?? User.FindFirstValue(JwtRegisteredClaimNames_Sub);
        return int.Parse(sub!);
    }
    private const string JwtRegisteredClaimNames_Sub = "sub";

    // =============================================
    // GET /api/GioHang — Lấy giỏ hàng của user
    // =============================================
    [HttpGet]
    public async Task<IActionResult> GetGioHang()
    {
        int maNd = GetMaNd();

        var gioHang = await _context.GioHangs
            .Where(g => g.MaNd == maNd)
            .Include(g => g.MaSachNavigation)
            .Select(g => new
            {
                maSach      = g.MaSach,
                tenSach     = g.MaSachNavigation.TenSach,
                hinhAnh     = g.MaSachNavigation.HinhAnh ?? "",
                giaGoc      = g.MaSachNavigation.GiaGoc,
                soLuong     = g.TongSlsach,
                slTon       = g.MaSachNavigation.Slton ?? 0
            })
            .ToListAsync();

        return Ok(gioHang);
    }

    // =============================================
    // POST /api/GioHang — Thêm sách vào giỏ
    // =============================================
    public class ThemVaoGioRequest
    {
        public int MaSach { get; set; }
        public int SoLuong { get; set; } = 1;
    }

    [HttpPost]
    public async Task<IActionResult> ThemVaoGio([FromBody] ThemVaoGioRequest request)
    {
        int maNd = GetMaNd();

        // Kiểm tra sách có tồn tại và còn hàng không
        var sach = await _context.Saches.FindAsync(request.MaSach);
        if (sach == null || sach.TrangThaiS == false)
            return NotFound(new { message = "Sách không tồn tại hoặc đã ngưng kinh doanh" });

        if (sach.Slton < request.SoLuong)
            return BadRequest(new { message = $"Sách chỉ còn {sach.Slton} cuốn trong kho" });

        // Kiểm tra đã có trong giỏ chưa
        var existing = await _context.GioHangs
            .FirstOrDefaultAsync(g => g.MaNd == maNd && g.MaSach == request.MaSach);

        if (existing != null)
        {
            // Đã có → cộng thêm số lượng
            existing.TongSlsach = (existing.TongSlsach ?? 0) + request.SoLuong;
        }
        else
        {
            // Chưa có → thêm mới
            _context.GioHangs.Add(new GioHang
            {
                MaNd       = maNd,
                MaSach     = request.MaSach,
                TongSlsach = request.SoLuong
            });
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Đã thêm vào giỏ hàng" });
    }

    // =============================================
    // PUT /api/GioHang/{maSach} — Cập nhật số lượng
    // =============================================
    public class CapNhatSoLuongRequest
    {
        public int SoLuong { get; set; }
    }

    [HttpPut("{maSach}")]
    public async Task<IActionResult> CapNhatSoLuong(int maSach, [FromBody] CapNhatSoLuongRequest request)
    {
        int maNd = GetMaNd();

        if (request.SoLuong < 1)
            return BadRequest(new { message = "Số lượng phải ít nhất là 1" });

        var item = await _context.GioHangs
            .FirstOrDefaultAsync(g => g.MaNd == maNd && g.MaSach == maSach);

        if (item == null)
            return NotFound(new { message = "Không tìm thấy sách trong giỏ hàng" });

        // Kiểm tra tồn kho
        var sach = await _context.Saches.FindAsync(maSach);
        if (sach != null && sach.Slton < request.SoLuong)
            return BadRequest(new { message = $"Sách chỉ còn {sach.Slton} cuốn trong kho" });

        item.TongSlsach = request.SoLuong;
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đã cập nhật số lượng" });
    }

    // =============================================
    // DELETE /api/GioHang/{maSach} — Xóa 1 sách khỏi giỏ
    // =============================================
    [HttpDelete("{maSach}")]
    public async Task<IActionResult> XoaKhoiGio(int maSach)
    {
        int maNd = GetMaNd();

        var item = await _context.GioHangs
            .FirstOrDefaultAsync(g => g.MaNd == maNd && g.MaSach == maSach);

        if (item == null)
            return NotFound(new { message = "Không tìm thấy sách trong giỏ hàng" });

        _context.GioHangs.Remove(item);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đã xóa khỏi giỏ hàng" });
    }

    // =============================================
    // DELETE /api/GioHang — Xóa toàn bộ giỏ hàng
    // =============================================
    [HttpDelete]
    public async Task<IActionResult> XoaToanBoGio()
    {
        int maNd = GetMaNd();

        var items = await _context.GioHangs
            .Where(g => g.MaNd == maNd)
            .ToListAsync();

        _context.GioHangs.RemoveRange(items);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đã xóa toàn bộ giỏ hàng" });
    }
}
