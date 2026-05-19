using QuanLyNhaSachAPI.DTOs;
using QuanLyNhaSachAPI.Repositories;
using QuanLyNhaSachAPI.Models;
using System.Text.Json;

namespace QuanLyNhaSachAPI.Services
{
    public interface IDonHangService
    {
        Task TaoDonHangAsync(int maNd, DonHangRequestDTO request);
        Task<List<DonHangAdminResponseDTO>> LayTatCaDonHangAsync();
        Task<List<DonHangKhachResponseDTO>> LayDonHangCuaKhachAsync(int maNd);
        Task<ChiTietDonHangResponseDTO> LayChiTietDonHangAsync(int id, int maNd, bool isAdmin);
        Task<int?> CapNhatTrangThaiAsync(int id, CapNhatTrangThaiDHRequestDTO request, int adminId);
    }

    public class DonHangService : IDonHangService
    {
        private readonly IDonHangRepository _donHangRepo;

        public DonHangService(IDonHangRepository donHangRepo)
        {
            _donHangRepo = donHangRepo;
        }

        public async Task TaoDonHangAsync(int maNd, DonHangRequestDTO request)
        {
            var khachHang = await _donHangRepo.GetKhachHangByMaNdAsync(maNd);
            if (khachHang == null)
                throw new ArgumentException("Tài khoản của bạn hiện tại chưa thể đặt hàng. Vui lòng thử lại sau!");

            string gioHangJson = JsonSerializer.Serialize(request.GioHang,
                new JsonSerializerOptions { PropertyNamingPolicy = null });

            await _donHangRepo.ThucThiTaoDonHangAsync(khachHang.MaNd, request.DiaChiGiaoHang, gioHangJson);
        }

        public async Task<List<DonHangAdminResponseDTO>> LayTatCaDonHangAsync()
        {
            return await _donHangRepo.GetAllDonHangProjectedAsync();
        }

        public async Task<List<DonHangKhachResponseDTO>> LayDonHangCuaKhachAsync(int maNd)
        {
            var khachHang = await _donHangRepo.GetKhachHangByMaNdAsync(maNd);
            if (khachHang == null)
                throw new ArgumentException("Không tìm thấy thông tin khách hàng.");

            return await _donHangRepo.GetDonHangsByKhachHangIdAsync(khachHang.MaNd);
        }

        public async Task<ChiTietDonHangResponseDTO> LayChiTietDonHangAsync(int id, int maNd, bool isAdmin)
        {
            var donHang = await _donHangRepo.GetDonHangByIdWithChiTietAsync(id);
            if (donHang == null)
                throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

            if (!isAdmin)
            {
                var khachHang = await _donHangRepo.GetKhachHangByMaNdAsync(maNd);
                if (khachHang == null || donHang.MaNd != khachHang.MaNd)
                    throw new UnauthorizedAccessException();
            }

            return new ChiTietDonHangResponseDTO
            {
                MaDh = donHang.MaDh,
                MaNd = donHang.MaNd ?? 0,
                NgayDat = donHang.NgayDat,
                TrangThaiDon = donHang.TrangThaiDon,
                DiaChiGh = donHang.DiaChiGh,
                TongTien = donHang.TongTien,
                MaVanDon = donHang.MaVanDon,
                MaNguoiDuyet = donHang.MaNguoiDuyet,
                ChiTiet = donHang.ChiTietDonDatHangs.Select(ct => new ChiTietItemDTO
                {
                    MaSach = ct.MaSach,
                    TenSach = ct.MaSachNavigation?.TenSach,
                    Slsach = ct.Slsach,
                    DonGia = ct.DonGia,
                    ThanhTien = ct.Slsach * ct.DonGia
                }).ToList()
            };
        }

        public async Task<int?> CapNhatTrangThaiAsync(int id, CapNhatTrangThaiDHRequestDTO request, int adminId)
        {
            var donHang = await _donHangRepo.GetDonHangByIdAsync(id);
            if (donHang == null)
                throw new KeyNotFoundException("Không tìm thấy đơn hàng.");

            bool isDuyetDon = request.TrangThaiMoi == 1 || request.TrangThaiMoi == 2;
            bool khongCoMaVanDon = string.IsNullOrWhiteSpace(request.MaVanDon) && string.IsNullOrWhiteSpace(donHang.MaVanDon);

            if (isDuyetDon && khongCoMaVanDon)
                throw new InvalidOperationException("Không thể chuyển trạng thái duyệt khi chưa có Mã vận đơn!");

            if (request.TrangThaiMoi == 3 && donHang.TrangThaiDon != 2)
                throw new InvalidOperationException("Lỗi quy trình: Không thể hoàn thành đơn hàng khi đơn chưa được chuyển sang trạng thái Đang giao hàng!");

            if (adminId != 0)
                donHang.MaNguoiDuyet = adminId;

            donHang.TrangThaiDon = request.TrangThaiMoi;
            donHang.MaVanDon = request.MaVanDon;

            await _donHangRepo.UpdateDonHangAsync(donHang);
            return donHang.TrangThaiDon;
        }
    }
}