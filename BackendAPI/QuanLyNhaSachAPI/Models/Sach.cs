using System;
using System.Collections.Generic;

namespace QuanLyNhaSachAPI.Models;

public partial class Sach
{
    public int MaSach { get; set; }

    public string TenSach { get; set; } = null!;

    public decimal GiaGoc { get; set; }

    public string? HinhAnh { get; set; }

    public string? MoTa { get; set; }

    public int? Slton { get; set; }

    public bool? TrangThaiS { get; set; }

    public int? MaTl { get; set; }

    public int? MaTg { get; set; }

    public int? MaNxb { get; set; }

    public virtual ICollection<ChiTietDonDatHang> ChiTietDonDatHangs { get; set; } = new List<ChiTietDonDatHang>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual NhaXuatBan? MaNxbNavigation { get; set; }

    public virtual TacGium? MaTgNavigation { get; set; }

    public virtual TheLoai? MaTlNavigation { get; set; }
}
