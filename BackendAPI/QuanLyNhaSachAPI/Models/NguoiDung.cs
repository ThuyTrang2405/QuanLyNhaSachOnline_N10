using System;
using System.Collections.Generic;

namespace QuanLyNhaSachAPI.Models;

public partial class NguoiDung
{
    public int MaNd { get; set; }

    public string HoTen { get; set; } = null!;

    public string? GioiTinh { get; set; }

    public string Email { get; set; } = null!;

    public string? Sdt { get; set; }

    public string? DiaChi { get; set; }

    public string TenNd { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public virtual KhachHang? KhachHang { get; set; }

    public virtual NguoiQuanTri? NguoiQuanTri { get; set; }
}
