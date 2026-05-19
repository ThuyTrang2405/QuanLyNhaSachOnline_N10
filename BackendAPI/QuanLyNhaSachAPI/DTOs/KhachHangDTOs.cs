namespace QuanLyNhaSachAPI.DTOs
{
    public class KhachHangResponseDTO
    {
        public int MaNd { get; set; }
        public string HoTen { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Sdt { get; set; } = "";
        public string DiaChi { get; set; } = "";
        public bool TrangThaiTk { get; set; }
    }

    public class CapNhatTrangThaiRequestDTO
    {
        public bool TrangThai { get; set; }
    }
}