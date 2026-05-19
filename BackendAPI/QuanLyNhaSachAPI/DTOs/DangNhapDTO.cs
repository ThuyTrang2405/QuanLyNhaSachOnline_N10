using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaSachAPI.DTOs;

public class DangNhapDTO
{
    [Required(ErrorMessage = "Email hoặc tên đăng nhập không được để trống")]
    public string TaiKhoan { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    public string MatKhau { get; set; } = null!;
}
