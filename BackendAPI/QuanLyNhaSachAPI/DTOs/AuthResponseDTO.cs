namespace QuanLyNhaSachAPI.DTOs;

public class AuthResponseDTO
{
    // JWT token trả về cho frontend lưu vào localStorage
    public string Token { get; set; } = null!;

    // Thông tin cơ bản của user (không trả về mật khẩu)
    public int MaNd { get; set; }
    public string HoTen { get; set; } = null!;
    public string TenDangNhap { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string VaiTro { get; set; } = null!; // "KhachHang" hoặc "QuanTri"
}
