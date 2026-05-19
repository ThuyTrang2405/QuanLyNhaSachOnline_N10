using Microsoft.EntityFrameworkCore;
using QuanLyNhaSachAPI.Models;

namespace QuanLyNhaSachAPI.Repositories
{
    public interface IAuthRepository
    {
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task CreateUserKhachHangAsync(NguoiDung nguoiDung, KhachHang khachHang);
        Task<NguoiDung?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
        Task<List<NguoiDung>> GetAllUsersAsync();
        Task UpdateUsersAsync();
    }

    public class AuthRepository : IAuthRepository
    {
        private readonly QuanLyNhaSachContext _context;

        public AuthRepository(QuanLyNhaSachContext context)
        {
            _context = context;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.NguoiDungs.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.NguoiDungs.AnyAsync(u => u.TenNd == username);
        }

        public async Task CreateUserKhachHangAsync(NguoiDung nguoiDung, KhachHang khachHang)
        {
            _context.NguoiDungs.Add(nguoiDung);
            await _context.SaveChangesAsync();

            khachHang.MaNd = nguoiDung.MaNd;
            _context.KhachHangs.Add(khachHang);
            await _context.SaveChangesAsync();
        }

        public async Task<NguoiDung?> GetUserByUsernameOrEmailAsync(string usernameOrEmail)
        {
            return await _context.NguoiDungs
                .Include(u => u.KhachHang)
                .Include(u => u.NguoiQuanTri)
                .FirstOrDefaultAsync(u => u.Email == usernameOrEmail || u.TenNd == usernameOrEmail);
        }

        public async Task<List<NguoiDung>> GetAllUsersAsync()
        {
            return await _context.NguoiDungs.ToListAsync();
        }

        public async Task UpdateUsersAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}