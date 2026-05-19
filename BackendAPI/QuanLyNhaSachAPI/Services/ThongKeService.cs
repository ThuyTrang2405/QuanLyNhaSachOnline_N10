using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Repositories;

namespace QuanLyNhaSachAPI.Services
{
    public interface IThongKeService
    {
        Task<List<DoanhThuDTO>> LayDoanhThuThangAsync();
        Task<TongQuanDTO> LayTongQuanAsync();
        Task<List<DoanhThuNgayDTO>> LayDoanhThuNgayAsync();
        Task<List<TopSachDTO>> LayTopSachBanChayAsync();
    }

    public class ThongKeService : IThongKeService
    {
        private readonly IThongKeRepository _thongKeRepo;

        public ThongKeService(IThongKeRepository thongKeRepo)
        {
            _thongKeRepo = thongKeRepo;
        }

        public async Task<List<DoanhThuDTO>> LayDoanhThuThangAsync()
        {
            return await _thongKeRepo.GetDoanhThuThangAsync();
        }

        public async Task<TongQuanDTO> LayTongQuanAsync()
        {
            return await _thongKeRepo.GetTongQuanAsync();
        }

        public async Task<List<DoanhThuNgayDTO>> LayDoanhThuNgayAsync()
        {
            return await _thongKeRepo.GetDoanhThuNgayAsync();
        }

        public async Task<List<TopSachDTO>> LayTopSachBanChayAsync()
        {
            return await _thongKeRepo.GetTopSachBanChayAsync();
        }
    }
}