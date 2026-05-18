using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QuanLyNhaSachAPI.Models;
using System.Data;
using System.Security.Claims;
using System.Text.Json;

namespace QuanLyNhaSachAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class DonHangController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly QuanLyNhaSachContext _context;

        public DonHangController(IConfiguration configuration, QuanLyNhaSachContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public class GioHangItem
        {
            public int MaSach { get; set; }
            public int SoLuongSP { get; set; }   
            public decimal DonGia { get; set; }
        }

        public class DonHangRequest
        {
            public string DiaChiGiaoHang { get; set; } = "";   
            public List<GioHangItem> GioHang { get; set; } = new(); 
        }

        [HttpPost("tao-don")]
        public async Task<IActionResult> TaoDonHang([FromBody] DonHangRequest request)
        {
            string? maNdChuoi = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(maNdChuoi))
            {
                return Unauthorized(new { message = "Không xác định được danh tính người dùng." });
            }

            int maNd = int.Parse(maNdChuoi);


            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNd == maNd);
            if (khachHang == null)
                return BadRequest(new { message = "Tài khoản của bạn hiện tại chưa thể đặt hàng. Vui lòng thử lại sau!" });
            string gioHangJson = JsonSerializer.Serialize(request.GioHang,
                new JsonSerializerOptions { PropertyNamingPolicy = null });

            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_TaoDonHang_CoTransaction", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 30; 

                    cmd.Parameters.AddWithValue("@MaKH", khachHang.MaNd);
                    cmd.Parameters.AddWithValue("@DiaChiGiaoHang", request.DiaChiGiaoHang);
                    cmd.Parameters.AddWithValue("@GioHangJSON",    gioHangJson);

                    try
                    {
                        await con.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        return Ok(new { message = "Đặt hàng thành công!" });
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 50001)
                        {
                            return BadRequest(new { message = ex.Message });
                        }

                        return StatusCode(500, new { message = "Hệ thống đang quá tải. Vui lòng thử lại sau giây lát!" });
                    }
                }
            }
        }
        // =============================================
        // 1. GET /api/DonHang - Danh sách đơn hàng (Chỉ Admin)
        // =============================================
        [HttpGet]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> GetAllDonHang()
        {
            var donHangs = await _context.DonDatHangs
                .Join(_context.NguoiDungs,         
                      don => don.MaNd,             
                      nd => nd.MaNd,               
                      (don, nd) => new { DonHang = don, NguoiDung = nd }) 
                .OrderByDescending(x => x.DonHang.NgayDat)
                .Select(x => new {
                    x.DonHang.MaDh,
                    x.DonHang.MaNd,
                    TenKhachHang = x.NguoiDung.HoTen,
                    x.DonHang.MaNguoiDuyet,
                    x.DonHang.NgayDat,
                    x.DonHang.TrangThaiDon,
                    x.DonHang.TongTien,
                    x.DonHang.DiaChiGh,
                    x.DonHang.MaVanDon
                })
                .ToListAsync();

            return Ok(donHangs);
        }

        // =============================================
        // 2. GET /api/DonHang/cua-toi - Lịch sử đơn của khách hàng
        // =============================================
        [HttpGet("cua-toi")]
        public async Task<IActionResult> GetDonHangCuaToi()
        {
            string? maNdChuoi = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(maNdChuoi))
            {
                return Unauthorized(new { message = "Không xác định được danh tính người dùng." });
            }

            int maNd = int.Parse(maNdChuoi);

            var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNd == maNd);
            if (khachHang == null)
                return BadRequest(new { message = "Không tìm thấy thông tin khách hàng." });

            var donHangs = await _context.DonDatHangs
                .Where(d => d.MaNd == khachHang.MaNd)
                .OrderByDescending(d => d.NgayDat)
                .Select(d => new {
                    d.MaDh,
                    d.NgayDat,
                    d.TrangThaiDon,
                    d.TongTien
                })
                .ToListAsync();

            return Ok(donHangs);
        }

        // =============================================
        // 3. GET /api/DonHang/{id} - Chi tiết 1 đơn hàng
        // =============================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChiTietDonHang(int id)
        {
            var donHang = await _context.DonDatHangs
                .Include(d => d.ChiTietDonDatHangs)
                    .ThenInclude(ct => ct.MaSachNavigation) // Join lấy tên sách
                .FirstOrDefaultAsync(d => d.MaDh == id);

            if (donHang == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            var isUserAdmin = User.IsInRole("QuanTri");
            if (!isUserAdmin)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub);
                int maNd = int.Parse(userIdClaim!.Value);
                var khachHang = await _context.KhachHangs.FirstOrDefaultAsync(k => k.MaNd == maNd);

                if (khachHang == null || donHang.MaNd != khachHang.MaNd)
                    return Forbid(); 
            }

            var result = new
            {
                donHang.MaDh,
                donHang.MaNd,
                donHang.NgayDat,
                donHang.TrangThaiDon,
                donHang.DiaChiGh,
                donHang.TongTien,
                donHang.MaVanDon,
                donHang.MaNguoiDuyet,
                ChiTiet = donHang.ChiTietDonDatHangs.Select(ct => new {
                    ct.MaSach,
                    TenSach = ct.MaSachNavigation?.TenSach,
                    ct.Slsach,
                    ct.DonGia,
                    ThanhTien = ct.Slsach * ct.DonGia
                })
            };

            return Ok(result);
        }

        public class CapNhatTrangThaiDHRequest
        {
            public int TrangThaiMoi { get; set; }
            public string? MaVanDon { get; set; } 
        }

        // =============================================
        // 4. PUT /api/DonHang/{id}/trang-thai - Cập nhật trạng thái (Chỉ Admin)
        // =============================================
        [HttpPut("{id}/trang-thai")]
        [Authorize(Roles = "QuanTri")]
        public async Task<IActionResult> UpdateTrangThai(int id, [FromBody] CapNhatTrangThaiDHRequest request)
        {
            var donHang = await _context.DonDatHangs.FindAsync(id);
            if (donHang == null)
                return NotFound(new { message = "Không tìm thấy đơn hàng." });

            bool isDuyetDon = request.TrangThaiMoi == 1 || request.TrangThaiMoi == 2;
            bool khongCoMaVanDon = string.IsNullOrWhiteSpace(request.MaVanDon) && string.IsNullOrWhiteSpace(donHang.MaVanDon);

            if (isDuyetDon && khongCoMaVanDon)
            {
                return BadRequest(new { message = "Không thể chuyển trạng thái duyệt khi chưa có Mã vận đơn!" });
            }

            if (request.TrangThaiMoi == 3 && donHang.TrangThaiDon != 2)
            {
                return BadRequest(new { message = "Lỗi quy trình: Không thể hoàn thành đơn hàng khi đơn chưa được chuyển sang trạng thái Đang giao hàng!" });
            }

            string? adminIdChuoi = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(adminIdChuoi))
            {
                donHang.MaNguoiDuyet = int.Parse(adminIdChuoi);
            }

            donHang.TrangThaiDon = request.TrangThaiMoi;

            donHang.MaVanDon = request.MaVanDon; 

            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái đơn hàng thành công!", trangThaiMoi = donHang.TrangThaiDon });
        }
    }

}