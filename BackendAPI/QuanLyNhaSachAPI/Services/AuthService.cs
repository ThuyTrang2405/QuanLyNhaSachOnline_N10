using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Models;
using QuanLyNhaSachAPI.Repositories;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QuanLyNhaSachAPI.Services
{
    public interface IAuthService
    {
        Task DangKyAsync(DangKyDTO dto);
        Task<AuthResponseDTO> DangNhapAsync(DangNhapDTO dto);
        Task<int> HashAllPasswordsAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepo;
        private readonly IConfiguration _config;

        public AuthService(IAuthRepository authRepo, IConfiguration config)
        {
            _authRepo = authRepo;
            _config = config; 
        }

        public async Task DangKyAsync(DangKyDTO dto)
        {
            if (await _authRepo.EmailExistsAsync(dto.Email))
                throw new InvalidOperationException("Email này đã được sử dụng");

            if (await _authRepo.UsernameExistsAsync(dto.TenDangNhap))
                throw new InvalidOperationException("Tên đăng nhập này đã được sử dụng");

            string matKhauHash = BCrypt.Net.BCrypt.HashPassword(dto.MatKhau);

            var nguoiDung = new NguoiDung
            {
                HoTen = dto.HoTen,
                Email = dto.Email,
                TenNd = dto.TenDangNhap,
                MatKhau = matKhauHash,
                Sdt = dto.Sdt,
                GioiTinh = dto.GioiTinh
            };

            var khachHang = new KhachHang
            {
                TrangThaiTk = true 
            };

            await _authRepo.CreateUserKhachHangAsync(nguoiDung, khachHang);
        }

        public async Task<AuthResponseDTO> DangNhapAsync(DangNhapDTO dto)
        {
            var nguoiDung = await _authRepo.GetUserByUsernameOrEmailAsync(dto.TaiKhoan);

            if (nguoiDung == null)
                throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không đúng");

            bool matKhauDung = BCrypt.Net.BCrypt.Verify(dto.MatKhau, nguoiDung.MatKhau);
            if (!matKhauDung)
                throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không đúng");

            if (nguoiDung.KhachHang != null && nguoiDung.KhachHang.TrangThaiTk == false)
                throw new UnauthorizedAccessException("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");

            string vaiTro = nguoiDung.NguoiQuanTri != null ? "QuanTri" : "KhachHang";
            string token = TaoJwtToken(nguoiDung, vaiTro);

            return new AuthResponseDTO
            {
                Token = token,
                MaNd = nguoiDung.MaNd,
                HoTen = nguoiDung.HoTen,
                TenDangNhap = nguoiDung.TenNd,
                Email = nguoiDung.Email,
                VaiTro = vaiTro
            };
        }

        public async Task<int> HashAllPasswordsAsync()
        {
            var users = await _authRepo.GetAllUsersAsync();
            int count = 0;

            foreach (var u in users)
            {
                if (!u.MatKhau.StartsWith("$2a$") && !u.MatKhau.StartsWith("$2b$"))
                {
                    u.MatKhau = BCrypt.Net.BCrypt.HashPassword(u.MatKhau);
                    count++;
                }
            }

            if (count > 0)
                await _authRepo.UpdateUsersAsync();

            return count;
        }

        private string TaoJwtToken(NguoiDung nguoiDung, string vaiTro)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            int expMinutes = int.Parse(_config["Jwt:ExpiresInMinutes"] ?? "60");

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
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}