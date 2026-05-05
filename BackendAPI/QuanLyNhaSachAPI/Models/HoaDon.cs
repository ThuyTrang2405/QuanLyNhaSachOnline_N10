using System;
using System.Collections.Generic;

namespace QuanLyNhaSachAPI.Models;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public DateTime? NgayTao { get; set; }

    public string? TrangThaiTt { get; set; }

    public string? GhiChu { get; set; }

    public int MaDh { get; set; }

    public virtual DonDatHang MaDhNavigation { get; set; } = null!;
}
