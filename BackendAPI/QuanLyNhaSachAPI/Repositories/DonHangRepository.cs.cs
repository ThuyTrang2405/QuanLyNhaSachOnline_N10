using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaSachAPI.Models;
using QuanLyNhaSachAPI.DTOs;
using System.Data;

namespace QuanLyNhaSachAPI.Repositories
{
    public interface IDonHangRepository
    {
        Task<KhachHang?> GetKhachHangByMaNdAsync(int maNd);
        Task ThucThiTaoDonHangAsync(int maKhachHang, string diaChiGiaoHang, string gioHangJson);
        Task<List<DonHangAdminResponseDTO>> GetAllDonHangProjectedAsync();
        Task<List<DonHangKhachResponseDTO>> GetDonHangsByKhachHangIdAsync(int maKhachHangId);
        Task<DonDatHang?> GetDonHangByIdWithChiTietAsync(int id);
        Task<DonDatHang?> GetDonHangByIdAsync(int id);
        Task UpdateDonHangAsync(DonDatHang donHang);
    }

    public class DonHangRepository : IDonHangRepository
    {
        private readonly QuanLyNhaSachContext _context;
        private readonly IConfiguration _configuration;

        public DonHangRepository(QuanLyNhaSachContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<KhachHang?> GetKhachHangByMaNdAsync(int maNd)
        {
            return await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNd == maNd);
        }

        public async Task ThucThiTaoDonHangAsync(int maKhachHang, string diaChiGiaoHang, string gioHangJson)
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_TaoDonHang_CoTransaction", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;

                    cmd.Parameters.AddWithValue("@MaKH", maKhachHang);
                    cmd.Parameters.AddWithValue("@DiaChiGiaoHang", diaChiGiaoHang);
                    cmd.Parameters.AddWithValue("@GioHangJSON", gioHangJson);

                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<DonHangAdminResponseDTO>> GetAllDonHangProjectedAsync()
        {
            return await _context.DonDatHangs
                .Join(_context.NguoiDungs,
                      don => don.MaNd,
                      nd => nd.MaNd,
                      (don, nd) => new { DonHang = don, NguoiDung = nd })
                .OrderByDescending(x => x.DonHang.NgayDat)
                .Select(x => new DonHangAdminResponseDTO
                {
                    MaDh = x.DonHang.MaDh,
                    MaNd = x.DonHang.MaNd ?? 0,
                    TenKhachHang = x.NguoiDung.HoTen,
                    MaNguoiDuyet = x.DonHang.MaNguoiDuyet,
                    NgayDat = x.DonHang.NgayDat,
                    TrangThaiDon = x.DonHang.TrangThaiDon,
                    TongTien = x.DonHang.TongTien,
                    DiaChiGh = x.DonHang.DiaChiGh,
                    MaVanDon = x.DonHang.MaVanDon
                })
                .ToListAsync();
        }

        public async Task<List<DonHangKhachResponseDTO>> GetDonHangsByKhachHangIdAsync(int maKhachHangId)
        {
            return await _context.DonDatHangs
                .Where(d => d.MaNd == maKhachHangId)
                .OrderByDescending(d => d.NgayDat)
                .Select(d => new DonHangKhachResponseDTO
                {
                    MaDh = d.MaDh,
                    NgayDat = d.NgayDat,
                    TrangThaiDon = d.TrangThaiDon,
                    TongTien = d.TongTien
                })
                .ToListAsync();
        }

        public async Task<DonDatHang?> GetDonHangByIdWithChiTietAsync(int id)
        {
            return await _context.DonDatHangs
                .Include(d => d.ChiTietDonDatHangs)
                    .ThenInclude(ct => ct.MaSachNavigation)
                .FirstOrDefaultAsync(d => d.MaDh == id);
        }

        public async Task<DonDatHang?> GetDonHangByIdAsync(int id)
        {
            return await _context.DonDatHangs.FindAsync(id);
        }

        public async Task UpdateDonHangAsync(DonDatHang donHang)
        {
            _context.DonDatHangs.Update(donHang);
            await _context.SaveChangesAsync();
        }
    }
}