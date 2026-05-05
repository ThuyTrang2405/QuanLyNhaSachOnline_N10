using System;
using System.Collections.Generic;

namespace QuanLyNhaSachAPI.Models;

public partial class GioHang
{
    public int MaNd { get; set; }

    public int MaSach { get; set; }

    public int? TongSlsach { get; set; }

    public virtual KhachHang MaNdNavigation { get; set; } = null!;

    public virtual Sach MaSachNavigation { get; set; } = null!;
}
