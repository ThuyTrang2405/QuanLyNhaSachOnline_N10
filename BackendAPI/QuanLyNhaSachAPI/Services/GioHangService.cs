using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Models;
using QuanLyNhaSachAPI.Repositories;

namespace QuanLyNhaSachAPI.Services
{
    public interface IGioHangService
    {
        Task<List<GioHangResponseDTO>> LayGioHangAsync(int maNd);
        Task ThemVaoGioAsync(int maNd, ThemVaoGioRequestDTO request);
        Task CapNhatSoLuongAsync(int maNd, int maSach, int soLuong);
        Task XoaKhoiGioAsync(int maNd, int maSach);
        Task XoaToanBoGioAsync(int maNd);
    }

    public class GioHangService : IGioHangService
    {
        private readonly IGioHangRepository _gioHangRepo;

        public GioHangService(IGioHangRepository gioHangRepo)
        {
            _gioHangRepo = gioHangRepo;
        }

        public async Task<List<GioHangResponseDTO>> LayGioHangAsync(int maNd)
        {
            return await _gioHangRepo.GetGioHangByMaNdAsync(maNd);
        }

        public async Task ThemVaoGioAsync(int maNd, ThemVaoGioRequestDTO request)
        {
            var sach = await _gioHangRepo.GetSachByIdAsync(request.MaSach);
            if (sach == null || sach.TrangThaiS == false)
                throw new KeyNotFoundException("Sách không tồn tại hoặc đã ngưng kinh doanh");

            if (sach.Slton < request.SoLuong)
                throw new InvalidOperationException($"Sách chỉ còn {sach.Slton} cuốn trong kho");

            var existing = await _gioHangRepo.GetGioHangItemAsync(maNd, request.MaSach);

            if (existing != null)
            {
                existing.TongSlsach = (existing.TongSlsach ?? 0) + request.SoLuong;
                await _gioHangRepo.UpdateItemAsync(existing);
            }
            else
            {
                var newItem = new GioHang
                {
                    MaNd = maNd,
                    MaSach = request.MaSach,
                    TongSlsach = request.SoLuong
                };
                await _gioHangRepo.AddItemAsync(newItem);
            }
        }

        public async Task CapNhatSoLuongAsync(int maNd, int maSach, int soLuong)
        {
            if (soLuong < 1)
                throw new ArgumentException("Số lượng phải ít nhất là 1");

            var item = await _gioHangRepo.GetGioHangItemAsync(maNd, maSach);
            if (item == null)
                throw new KeyNotFoundException("Không tìm thấy sách trong giỏ hàng");

            var sach = await _gioHangRepo.GetSachByIdAsync(maSach);
            if (sach != null && sach.Slton < soLuong)
                throw new InvalidOperationException($"Sách chỉ còn {sach.Slton} cuốn trong kho");

            item.TongSlsach = soLuong;
            await _gioHangRepo.UpdateItemAsync(item);
        }

        public async Task XoaKhoiGioAsync(int maNd, int maSach)
        {
            var item = await _gioHangRepo.GetGioHangItemAsync(maNd, maSach);
            if (item == null)
                throw new KeyNotFoundException("Không tìm thấy sách trong giỏ hàng");

            await _gioHangRepo.RemoveItemAsync(item);
        }

        public async Task XoaToanBoGioAsync(int maNd)
        {
            await _gioHangRepo.ClearGioHangAsync(maNd);
        }
    }
}