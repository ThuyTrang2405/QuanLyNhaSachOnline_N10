using System;
using System.Collections.Generic;

namespace QuanLyNhaSachAPI.DTOs
{
    public class GioHangItemDTO
    {
        public int MaSach { get; set; }
        public int SoLuongSP { get; set; }
        public decimal DonGia { get; set; }
    }

    public class DonHangRequestDTO
    {
        public string DiaChiGiaoHang { get; set; } = "";
        public List<GioHangItemDTO> GioHang { get; set; } = new();
    }

    public class CapNhatTrangThaiDHRequestDTO
    {
        public int TrangThaiMoi { get; set; }
        public string? MaVanDon { get; set; }
    }

    public class DonHangAdminResponseDTO
    {
        public int MaDh { get; set; }
        public int MaNd { get; set; }
        public string TenKhachHang { get; set; } = "";
        public int? MaNguoiDuyet { get; set; }
        public DateTime? NgayDat { get; set; }
        public int? TrangThaiDon { get; set; }
        public decimal? TongTien { get; set; }
        public string? DiaChiGh { get; set; }
        public string? MaVanDon { get; set; }
    }

    public class DonHangKhachResponseDTO
    {
        public int MaDh { get; set; }
        public DateTime? NgayDat { get; set; }
        public int? TrangThaiDon { get; set; }
        public decimal? TongTien { get; set; }
    }

    public class ChiTietDonHangResponseDTO
    {
        public int MaDh { get; set; }
        public int MaNd { get; set; }
        public DateTime? NgayDat { get; set; }
        public int? TrangThaiDon { get; set; }
        public string? DiaChiGh { get; set; }
        public decimal? TongTien { get; set; }
        public string? MaVanDon { get; set; }
        public int? MaNguoiDuyet { get; set; }
        public List<ChiTietItemDTO> ChiTiet { get; set; } = new();
    }

    public class ChiTietItemDTO
    {
        public int MaSach { get; set; }
        public string? TenSach { get; set; }
        public int? Slsach { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien { get; set; }
    }
}