using Microsoft.EntityFrameworkCore;
using QuanLyNhaSachAPI.Models;
using QuanLyNhaSachAPI.DTOs;

namespace QuanLyNhaSachAPI.Repositories
{
    public interface IKhachHangRepository
    {
        Task<List<KhachHangResponseDTO>> GetAllKhachHangsAsync();
        Task<KhachHang?> GetKhachHangByIdAsync(int id);
        Task UpdateKhachHangAsync(KhachHang khachHang);
    }

    public class KhachHangRepository : IKhachHangRepository
    {
        private readonly QuanLyNhaSachContext _context;

        public KhachHangRepository(QuanLyNhaSachContext context)
        {
            _context = context;
        }

        public async Task<List<KhachHangResponseDTO>> GetAllKhachHangsAsync()
        {
            return await _context.KhachHangs
                .Include(kh => kh.MaNdNavigation)
                .Select(kh => new KhachHangResponseDTO
                {
                    MaNd = kh.MaNd,
                    HoTen = kh.MaNdNavigation.HoTen,
                    Email = kh.MaNdNavigation.Email,
                    Sdt = kh.MaNdNavigation.Sdt ?? "",
                    DiaChi = kh.MaNdNavigation.DiaChi ?? "",
                    TrangThaiTk = kh.TrangThaiTk ?? true
                })
                .OrderBy(kh => kh.HoTen)
                .ToListAsync();
        }

        public async Task<KhachHang?> GetKhachHangByIdAsync(int id)
        {
            return await _context.KhachHangs.FindAsync(id);
        }

        public async Task UpdateKhachHangAsync(KhachHang khachHang)
        {
            _context.KhachHangs.Update(khachHang);
            await _context.SaveChangesAsync();
        }
    }
}