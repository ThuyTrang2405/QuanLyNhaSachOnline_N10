using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaSachAPI.Models;

public partial class DonDatHang
{
    public int MaDh { get; set; }

    public DateTime? NgayDat { get; set; }

    public int? TrangThaiDon { get; set; }

    public string? MaVanDon { get; set; }

    public string DiaChiGh { get; set; } = null!;

    public int? MaNd { get; set; }

    public int? MaNguoiDuyet { get; set; }

    [ForeignKey("MaNguoiDuyet")]
    public virtual NguoiQuanTri NguoiQuanTri { get; set; }

    public decimal? TongTien { get; set; }
    public virtual ICollection<ChiTietDonDatHang> ChiTietDonDatHangs { get; set; } = new List<ChiTietDonDatHang>();

    public virtual HoaDon? HoaDon { get; set; }

    public virtual KhachHang? MaNdNavigation { get; set; }
}
