using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuanLyNhaSachAPI.Models;
using System.Data;

namespace QuanLyNhaSachAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "QuanTri")] // chỉ Admin mới được xem thống kê
    public class ThongKeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly QuanLyNhaSachContext _context;

        public ThongKeController(IConfiguration configuration, QuanLyNhaSachContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpGet("doanh-thu")]
        public async Task<IActionResult> GetDoanhThu()
        {
            var result = new List<Dictionary<string, object>>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ThongKeDoanhThu", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30;

                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new Dictionary<string, object>
                            {
                                { "nam",        reader["Nam"] },
                                { "thang",      reader["Thang"] },
                                { "soDonHang",  reader["SoDonHang"] },
                                { "doanhThu",   reader["DoanhThu"] }
                            });
                        }
                    }
                }
            }
            return Ok(result);
        }

        [HttpGet("tong-quan")]
        public async Task<IActionResult> GetThongKeTongQuan()
        {
            // LINQ: Đếm tổng số đơn hàng Đã Hoàn Thành (Dùng CountAsync)
            var tongSoDon = await _context.DonDatHangs
                .CountAsync(d => d.TrangThaiDon == 3);

            // LINQ: Tính tổng doanh thu (Dùng SumAsync)
            var tongDoanhThu = await _context.DonDatHangs
                .Where(d => d.TrangThaiDon == 3)
                .SumAsync(d => d.TongTien) ?? 0;

            // LINQ: Đếm số lượng đầu sách đang kinh doanh (Dùng CountAsync)
            var tongSoSach = await _context.Saches
                .CountAsync(s => s.TrangThaiS == true);

            return Ok(new
            {
                SoDonHang = tongSoDon,
                DoanhThu = tongDoanhThu,
                SoSachDangBan = tongSoSach
            });
        }


        [HttpGet("doanh-thu-ngay")]
        public async Task<IActionResult> GetDoanhThuTheoNgay()
        {
            var result = new List<Dictionary<string, object>>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ThongKeDoanhThuTheoNgay", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Gọi Stored Procedure
                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new Dictionary<string, object>
                            {
                                { "ngay",       Convert.ToDateTime(reader["Ngay"]).ToString("dd/MM/yyyy") },
                                { "soDonHang",  reader["SoDonHang"] },
                                { "doanhThu",   reader["DoanhThu"] }
                            });
                        }
                    }
                }
            }
            return Ok(result);
        }


        [HttpGet("top-sach")]
        public async Task<IActionResult> GetTopSachBanChay()
        {
            var result = new List<Dictionary<string, object>>();
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                // Dùng câu lệnh SELECT trực tiếp để lấy dữ liệu từ View
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM vw_TopSachBanChay", con))
                {
                    cmd.CommandType = CommandType.Text; //Gọi View thì dùng Text, không dùng StoredProcedure
                    await con.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new Dictionary<string, object>
                            {
                                { "maSach",    reader["MaSach"] },
                                { "tenSach",   reader["TenSach"] },
                                { "tongDaBan", reader["TongSoLuongBan"] }
                            });
                        }
                    }
                }
            }
            return Ok(result);
        }
    }
}