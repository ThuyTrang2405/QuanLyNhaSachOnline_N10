using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuanLyNhaSachAPI.Services;

namespace QuanLyNhaSachAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "QuanTri")] 
    public class ThongKeController : ControllerBase
    {
        private readonly IThongKeService _thongKeService;

        public ThongKeController(IThongKeService thongKeService)
        {
            _thongKeService = thongKeService;
        }

        [HttpGet("doanh-thu")]
        public async Task<IActionResult> GetDoanhThu()
        {
            var result = await _thongKeService.LayDoanhThuThangAsync();
            return Ok(result);
        }

        [HttpGet("tong-quan")]
        public async Task<IActionResult> GetThongKeTongQuan()
        {
            var result = await _thongKeService.LayTongQuanAsync();
            return Ok(result);
        }

        [HttpGet("doanh-thu-ngay")]
        public async Task<IActionResult> GetDoanhThuTheoNgay()
        {
            var result = await _thongKeService.LayDoanhThuNgayAsync();
            return Ok(result);
        }

        [HttpGet("top-sach")]
        public async Task<IActionResult> GetTopSachBanChay()
        {
            var result = await _thongKeService.LayTopSachBanChayAsync();
            return Ok(result);
        }
    }
}