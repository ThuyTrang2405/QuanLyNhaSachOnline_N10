namespace QuanLyNhaSachAPI.DTOs;

public class AuthResponseDTO
{
    public string Token { get; set; } = null!;
    public int MaNd { get; set; }
    public string HoTen { get; set; } = null!;
    public string TenDangNhap { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string VaiTro { get; set; } = null!; 
}
