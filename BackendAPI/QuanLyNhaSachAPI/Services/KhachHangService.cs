using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Repositories;

namespace QuanLyNhaSachAPI.Services
{
    public interface IKhachHangService
    {
        Task<List<KhachHangResponseDTO>> LayDanhSachKhachHangAsync();
        Task<string> CapNhatTrangThaiAsync(int id, bool trangThai);
    }

    public class KhachHangService : IKhachHangService
    {
        private readonly IKhachHangRepository _khachHangRepo;

        public KhachHangService(IKhachHangRepository khachHangRepo)
        {
            _khachHangRepo = khachHangRepo;
        }

        public async Task<List<KhachHangResponseDTO>> LayDanhSachKhachHangAsync()
        {
            return await _khachHangRepo.GetAllKhachHangsAsync();
        }

        public async Task<string> CapNhatTrangThaiAsync(int id, bool trangThai)
        {
            var khachHang = await _khachHangRepo.GetKhachHangByIdAsync(id);

            if (khachHang == null)
                throw new KeyNotFoundException("Không tìm thấy khách hàng");

            khachHang.TrangThaiTk = trangThai;
            await _khachHangRepo.UpdateKhachHangAsync(khachHang);

            return trangThai ? "mở khóa" : "khóa";
        }
    }
}