using Microsoft.EntityFrameworkCore;
using QuanLyNhaSachAPI.Models;
using QuanLyNhaSachAPI.DTOs;

namespace QuanLyNhaSachAPI.Repositories
{
    public interface ITheLoaiRepository
    {
        Task<List<TheLoaiResponseDTO>> GetAllTheLoaisAsync();
        Task<TheLoai?> GetTheLoaiByIdAsync(int id);
        Task<bool> CheckTenTheLoaiTonTaiAsync(string tenTl, int? maTlHienTai = null);
        Task<bool> CheckTheLoaiDangDuocSuDungAsync(int id);
        Task AddTheLoaiAsync(TheLoai theLoai);
        Task UpdateTheLoaiAsync(TheLoai theLoai);
        Task DeleteTheLoaiAsync(TheLoai theLoai);
    }

    public class TheLoaiRepository : ITheLoaiRepository
    {
        private readonly QuanLyNhaSachContext _context;

        public TheLoaiRepository(QuanLyNhaSachContext context)
        {
            _context = context;
        }

        public async Task<List<TheLoaiResponseDTO>> GetAllTheLoaisAsync()
        {
            return await _context.TheLoais
                .OrderBy(t => t.TenTl)
                .Select(t => new TheLoaiResponseDTO
                {
                    MaTl = t.MaTl,
                    TenTl = t.TenTl
                })
                .ToListAsync();
        }

        public async Task<TheLoai?> GetTheLoaiByIdAsync(int id)
        {
            return await _context.TheLoais.FindAsync(id);
        }

        public async Task<bool> CheckTenTheLoaiTonTaiAsync(string tenTl, int? maTlHienTai = null)
        {
            if (maTlHienTai.HasValue)
            {
                return await _context.TheLoais.AnyAsync(t => t.TenTl == tenTl && t.MaTl != maTlHienTai.Value);
            }
            return await _context.TheLoais.AnyAsync(t => t.TenTl == tenTl);
        }

        public async Task<bool> CheckTheLoaiDangDuocSuDungAsync(int id)
        {
            return await _context.Saches.AnyAsync(s => s.MaTl == id);
        }

        public async Task AddTheLoaiAsync(TheLoai theLoai)
        {
            _context.TheLoais.Add(theLoai);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTheLoaiAsync(TheLoai theLoai)
        {
            _context.TheLoais.Update(theLoai);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTheLoaiAsync(TheLoai theLoai)
        {
            _context.TheLoais.Remove(theLoai);
            await _context.SaveChangesAsync();
        }
    }
}