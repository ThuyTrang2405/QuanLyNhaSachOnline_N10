using Microsoft.EntityFrameworkCore;
using QuanLyNhaSachAPI.Models;
using QuanLyNhaSachAPI.DTOs;

namespace QuanLyNhaSachAPI.Repositories
{
    public interface ISachRepository
    {
        Task<List<SachResponseDTO>> GetAllActiveSachesAsync();
        Task<SachResponseDTO?> GetActiveSachByIdAsync(int id);
        Task<Sach?> GetSachByIdAsync(int id);
        Task AddSachAsync(Sach sach);
        Task UpdateSachAsync(Sach sach);
    }

    public class SachRepository : ISachRepository
    {
        private readonly QuanLyNhaSachContext _context;

        public SachRepository(QuanLyNhaSachContext context)
        {
            _context = context;
        }

        public async Task<List<SachResponseDTO>> GetAllActiveSachesAsync()
        {
            return await _context.Saches
                .Where(s => s.TrangThaiS == true)
                .Include(s => s.MaTgNavigation)
                .Include(s => s.MaTlNavigation)
                .Select(s => new SachResponseDTO
                {
                    MaSach = s.MaSach,
                    TenSach = s.TenSach,
                    GiaGoc = s.GiaGoc,
                    HinhAnh = s.HinhAnh ?? "",
                    MoTa = s.MoTa ?? "",
                    SlTon = s.Slton ?? 0,
                    TenTacGia = s.MaTgNavigation != null ? s.MaTgNavigation.TenTg : "Đang cập nhật",
                    TenTheLoai = s.MaTlNavigation != null ? s.MaTlNavigation.TenTl : "Đang cập nhật",
                    MaTG = s.MaTg,
                    MaTL = s.MaTl
                })
                .ToListAsync();
        }

        public async Task<SachResponseDTO?> GetActiveSachByIdAsync(int id)
        {
            return await _context.Saches
                .Where(s => s.MaSach == id && s.TrangThaiS == true)
                .Include(s => s.MaTgNavigation)
                .Include(s => s.MaTlNavigation)
                .Select(s => new SachResponseDTO
                {
                    MaSach = s.MaSach,
                    TenSach = s.TenSach,
                    GiaGoc = s.GiaGoc,
                    HinhAnh = s.HinhAnh ?? "",
                    MoTa = s.MoTa ?? "",
                    SlTon = s.Slton ?? 0,
                    TenTacGia = s.MaTgNavigation != null ? s.MaTgNavigation.TenTg : "Đang cập nhật",
                    TenTheLoai = s.MaTlNavigation != null ? s.MaTlNavigation.TenTl : "Đang cập nhật",
                    MaTG = s.MaTg,
                    MaTL = s.MaTl
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Sach?> GetSachByIdAsync(int id)
        {
            return await _context.Saches.FirstOrDefaultAsync(s => s.MaSach == id);
        }

        public async Task AddSachAsync(Sach sach)
        {
            _context.Saches.Add(sach);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateSachAsync(Sach sach)
        {
            _context.Saches.Update(sach);
            await _context.SaveChangesAsync();
        }
    }
}