namespace QuanLyNhaSachAPI.DTOs
{
    public class GioHangResponseDTO
    {
        public int MaSach { get; set; }
        public string? TenSach { get; set; }
        public string HinhAnh { get; set; } = "";
        public decimal? GiaGoc { get; set; }
        public int SoLuong { get; set; }
        public int SlTon { get; set; }
    }

    public class ThemVaoGioRequestDTO
    {
        public int MaSach { get; set; }
        public int SoLuong { get; set; } = 1;
    }

    public class CapNhatSoLuongRequestDTO
    {
        public int SoLuong { get; set; }
    }
}