using System;
using System.Collections.Generic;

namespace QuanLyNhaSachAPI.Models;

public partial class ChiTietDonDatHang
{
    public int MaDh { get; set; }

    public int MaSach { get; set; }

    public int Slsach { get; set; }

    public decimal DonGia { get; set; }

    public virtual DonDatHang MaDhNavigation { get; set; } = null!;

    public virtual Sach MaSachNavigation { get; set; } = null!;
}
