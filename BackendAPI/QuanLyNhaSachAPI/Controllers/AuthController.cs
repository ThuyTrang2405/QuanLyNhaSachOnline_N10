using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuanLyNhaSachAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly QuanLyNhaSachContext _context;
    private readonly IConfiguration _config;

    public AuthController(QuanLyNhaSachContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // =============================================
    // POST /api/Auth/register
    // =============================================
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] DangKyDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Kiểm tra email đã tồn tại chưa
        bool emailTonTai = await _context.NguoiDungs
            .AnyAsync(u => u.Email == dto.Email);
        if (emailTonTai)
            return Conflict(new { message = "Email này đã được sử dụng" });

        // Kiểm tra tên đăng nhập đã tồn tại chưa
        bool tenDangNhapTonTai = await _context.NguoiDungs
            .AnyAsync(u => u.TenNd == dto.TenDangNhap);
        if (tenDangNhapTonTai)
            return Conflict(new { message = "Tên đăng nhập này đã được sử dụng" });

        // Hash mật khẩu trước khi lưu
        string matKhauHash = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau);

        // Tạo bản ghi NguoiDung
        var nguoiDung = new NguoiDung
        {
            HoTen    = dto.HoTen,
            Email    = dto.Email,
            TenNd    = dto.TenDangNhap,
            MatKhau  = matKhauHash,
            Sdt      = dto.Sdt,
            GioiTinh = dto.GioiTinh
        };

        _context.NguoiDungs.Add(nguoiDung);
        await _context.SaveChangesAsync(); // lưu để có MaNd

        // Tạo bản ghi KhachHang kế thừa từ NguoiDung
        var khachHang = new KhachHang
        {
            MaNd       = nguoiDung.MaNd,
            TrangThaiTk = true // tài khoản mặc định đang hoạt động
        };

        _context.KhachHangs.Add(khachHang);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đăng ký thành công! Vui lòng đăng nhập." });
    }

    // =============================================
    // POST /api/Auth/login
    // =============================================
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] DangNhapDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Tìm user theo email hoặc tên đăng nhập
        var nguoiDung = await _context.NguoiDungs
            .Include(u => u.KhachHang)
            .Include(u => u.NguoiQuanTri)
            .FirstOrDefaultAsync(u =>
                u.Email == dto.TaiKhoan || u.TenNd == dto.TaiKhoan);

        if (nguoiDung == null)
            return Unauthorized(new { message = "Tài khoản hoặc mật khẩu không đúng" });

        // Verify mật khẩu với BCrypt
        bool matKhauDung = BCrypt.Net.BCrypt.Verify(dto.MatKhau, nguoiDung.MatKhau);
        if (!matKhauDung)
            return Unauthorized(new { message = "Tài khoản hoặc mật khẩu không đúng" });

        // Kiểm tra tài khoản có bị khóa không (chỉ áp dụng cho KhachHang)
        if (nguoiDung.KhachHang != null && nguoiDung.KhachHang.TrangThaiTk == false)
            return Unauthorized(new { message = "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên." });

        // Xác định vai trò
        string vaiTro = nguoiDung.NguoiQuanTri != null ? "QuanTri" : "KhachHang";

        // Tạo JWT token
        string token = TaoJwtToken(nguoiDung, vaiTro);

        var response = new AuthResponseDTO
        {
            Token        = token,
            MaNd         = nguoiDung.MaNd,
            HoTen        = nguoiDung.HoTen,
            TenDangNhap  = nguoiDung.TenNd,
            Email        = nguoiDung.Email,
            VaiTro       = vaiTro
        };

        return Ok(response);
    }

    // =============================================
    // POST /api/Auth/hash-all-passwords
    // Dùng 1 lần để hash toàn bộ mật khẩu plain text trong DB
    // Sau khi chạy xong nên xóa endpoint này đi
    // =============================================
    [HttpPost("hash-all-passwords")]
    [AllowAnonymous]
    public async Task<IActionResult> HashAllPasswords()
    {
        var users = await _context.NguoiDungs.ToListAsync();
        int count = 0;

        foreach (var u in users)
        {
            // Chỉ hash những mật khẩu chưa được hash
            // BCrypt hash luôn bắt đầu bằng $2a$ hoặc $2b$
            if (!u.MatKhau.StartsWith("$2a$") && !u.MatKhau.StartsWith("$2b$"))
            {
                u.MatKhau = BCrypt.Net.BCrypt.HashPassword(u.MatKhau);
                count++;
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = $"Hoàn tất! Đã hash {count} tài khoản. Các tài khoản đã hash trước đó được giữ nguyên." });
    }

    // =============================================
    // HELPER: Tạo JWT token
    // =============================================
    private string TaoJwtToken(NguoiDung nguoiDung, string vaiTro)
    {
        var key       = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds     = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        int expMinutes = int.Parse(_config["Jwt:ExpiresInMinutes"] ?? "60");

        // Các claims được nhúng vào token
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   nguoiDung.MaNd.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, nguoiDung.Email),
            new Claim("tenDangNhap",                 nguoiDung.TenNd),
            new Claim("hoTen",                       nguoiDung.HoTen),
            new Claim(ClaimTypes.Role,               vaiTro),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             _config["Jwt:Issuer"],
            audience:           _config["Jwt:Audience"],
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(expMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
