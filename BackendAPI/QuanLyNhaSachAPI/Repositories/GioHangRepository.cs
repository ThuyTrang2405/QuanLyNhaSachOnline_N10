using Microsoft.EntityFrameworkCore;
using QuanLyNhaSachAPI.Models;
using QuanLyNhaSachAPI.DTOs;

namespace QuanLyNhaSachAPI.Repositories
{
    public interface IGioHangRepository
    {
        Task<List<GioHangResponseDTO>> GetGioHangByMaNdAsync(int maNd);
        Task<Sach?> GetSachByIdAsync(int maSach);
        Task<GioHang?> GetGioHangItemAsync(int maNd, int maSach);
        Task AddItemAsync(GioHang item);
        Task UpdateItemAsync(GioHang item);
        Task RemoveItemAsync(GioHang item);
        Task ClearGioHangAsync(int maNd);
    }

    public class GioHangRepository : IGioHangRepository
    {
        private readonly QuanLyNhaSachContext _context;

        public GioHangRepository(QuanLyNhaSachContext context)
        {
            _context = context;
        }

        public async Task<List<GioHangResponseDTO>> GetGioHangByMaNdAsync(int maNd)
        {
            return await _context.GioHangs
                .Where(g => g.MaNd == maNd)
                .Include(g => g.MaSachNavigation)
                .Select(g => new GioHangResponseDTO
                {
                    MaSach = g.MaSach,
                    TenSach = g.MaSachNavigation.TenSach,
                    HinhAnh = g.MaSachNavigation.HinhAnh ?? "",
                    GiaGoc = g.MaSachNavigation.GiaGoc,
                    SoLuong = g.TongSlsach ?? 0,
                    SlTon = g.MaSachNavigation.Slton ?? 0
                })
                .ToListAsync();
        }

        public async Task<Sach?> GetSachByIdAsync(int maSach)
        {
            return await _context.Saches.FindAsync(maSach);
        }

        public async Task<GioHang?> GetGioHangItemAsync(int maNd, int maSach)
        {
            return await _context.GioHangs
                .FirstOrDefaultAsync(g => g.MaNd == maNd && g.MaSach == maSach);
        }

        public async Task AddItemAsync(GioHang item)
        {
            _context.GioHangs.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateItemAsync(GioHang item)
        {
            _context.GioHangs.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(GioHang item)
        {
            _context.GioHangs.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task ClearGioHangAsync(int maNd)
        {
            var items = await _context.GioHangs.Where(g => g.MaNd == maNd).ToListAsync();
            if (items.Any())
            {
                _context.GioHangs.RemoveRange(items);
                await _context.SaveChangesAsync();
            }
        }
    }
}