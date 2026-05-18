using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaSachAPI.DTOs;

public class DangKyDTO
{
    [Required(ErrorMessage = "Họ tên không được để trống")]
    [MaxLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    public string HoTen { get; set; } = null!;

    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [MaxLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
    [MinLength(4, ErrorMessage = "Tên đăng nhập phải có ít nhất 4 ký tự")]
    [MaxLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự")]
    public string TenDangNhap { get; set; } = null!;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    [MaxLength(255)]
    public string MatKhau { get; set; } = null!;

    [MaxLength(15, ErrorMessage = "Số điện thoại tối đa 15 ký tự")]
    public string? Sdt { get; set; }

    [MaxLength(10)]
    public string? GioiTinh { get; set; }
}
