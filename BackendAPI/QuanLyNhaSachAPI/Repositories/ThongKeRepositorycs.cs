using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaSachAPI.Models;
using QuanLyNhaSachAPI.DTOs;
using System.Data;

namespace QuanLyNhaSachAPI.Repositories
{
    public interface IThongKeRepository
    {
        Task<List<DoanhThuDTO>> GetDoanhThuThangAsync();
        Task<TongQuanDTO> GetTongQuanAsync();
        Task<List<DoanhThuNgayDTO>> GetDoanhThuNgayAsync();
        Task<List<TopSachDTO>> GetTopSachBanChayAsync();
    }

    public class ThongKeRepository : IThongKeRepository
    {
        private readonly QuanLyNhaSachContext _context;
        private readonly IConfiguration _configuration;

        public ThongKeRepository(QuanLyNhaSachContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<DoanhThuDTO>> GetDoanhThuThangAsync()
        {
            var result = new List<DoanhThuDTO>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ThongKeDoanhThu", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new DoanhThuDTO
                            {
                                Nam = Convert.ToInt32(reader["Nam"]),
                                Thang = Convert.ToInt32(reader["Thang"]),
                                SoDonHang = Convert.ToInt32(reader["SoDonHang"]),
                                DoanhThu = Convert.ToDecimal(reader["DoanhThu"])
                            });
                        }
                    }
                }
            }
            return result;
        }

        public async Task<TongQuanDTO> GetTongQuanAsync()
        {
            var tongSoDon = await _context.DonDatHangs.CountAsync(d => d.TrangThaiDon == 3);
            var tongDoanhThu = await _context.DonDatHangs.Where(d => d.TrangThaiDon == 3).SumAsync(d => d.TongTien) ?? 0;
            var tongSoSach = await _context.Saches.CountAsync(s => s.TrangThaiS == true);

            return new TongQuanDTO
            {
                SoDonHang = tongSoDon,
                DoanhThu = tongDoanhThu,
                SoSachDangBan = tongSoSach
            };
        }

        public async Task<List<DoanhThuNgayDTO>> GetDoanhThuNgayAsync()
        {
            var result = new List<DoanhThuNgayDTO>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ThongKeDoanhThuTheoNgay", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new DoanhThuNgayDTO
                            {
                                Ngay = Convert.ToDateTime(reader["Ngay"]).ToString("dd/MM/yyyy"),
                                SoDonHang = Convert.ToInt32(reader["SoDonHang"]),
                                DoanhThu = Convert.ToDecimal(reader["DoanhThu"])
                            });
                        }
                    }
                }
            }
            return result;
        }

        public async Task<List<TopSachDTO>> GetTopSachBanChayAsync()
        {
            var result = new List<TopSachDTO>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM vw_TopSachBanChay", con))
                {
                    cmd.CommandType = CommandType.Text;
                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new TopSachDTO
                            {
                                MaSach = Convert.ToInt32(reader["MaSach"]),
                                TenSach = reader["TenSach"].ToString() ?? "",
                                TongDaBan = Convert.ToInt32(reader["TongSoLuongBan"])
                            });
                        }
                    }
                }
            }
            return result;
        }
    }
}