namespace QuanLyNhaSachAPI.DTOs
{
    public class TheLoaiResponseDTO
    {
        public int MaTl { get; set; }
        public string TenTl { get; set; } = "";
    }

    public class TheLoaiRequestDTO
    {
        public string TenTl { get; set; } = "";
    }
}