namespace QuanLyNhaSachAPI.DTOs
{
    public class SachResponseDTO
    {
        public int MaSach { get; set; }
        public string? TenSach { get; set; }
        public decimal? GiaGoc { get; set; }
        public string HinhAnh { get; set; } = "";
        public string MoTa { get; set; } = "";
        public int SlTon { get; set; }
        public string TenTacGia { get; set; } = "";
        public string TenTheLoai { get; set; } = "";
        public int? MaTG { get; set; }
        public int? MaTL { get; set; }
    }

    public class SachRequestDTO
    {
        public string? TenSach { get; set; }
        public decimal? GiaGoc { get; set; }
        public string? HinhAnh { get; set; }
        public string? MoTa { get; set; }
        public int? Slton { get; set; }
        public int? MaTg { get; set; }
        public int? MaTl { get; set; }
    }
}