namespace QuanLyNhaSachAPI.DTOs
{
    public class DoanhThuDTO
    {
        public int Nam { get; set; }
        public int Thang { get; set; }
        public int SoDonHang { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class TongQuanDTO
    {
        public int SoDonHang { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoSachDangBan { get; set; }
    }

    public class DoanhThuNgayDTO
    {
        public string Ngay { get; set; } = "";
        public int SoDonHang { get; set; }
        public decimal DoanhThu { get; set; }
    }

    public class TopSachDTO
    {
        public int MaSach { get; set; }
        public string TenSach { get; set; } = "";
        public int TongDaBan { get; set; }
    }
}