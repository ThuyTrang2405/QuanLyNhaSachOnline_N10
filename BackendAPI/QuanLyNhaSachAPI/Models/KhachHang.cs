using System;
using System.Collections.Generic;

namespace QuanLyNhaSachAPI.Models;

public partial class KhachHang
{
    public int MaNd { get; set; }

    public bool? TrangThaiTk { get; set; }

    public virtual ICollection<DonDatHang> DonDatHangs { get; set; } = new List<DonDatHang>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual NguoiDung MaNdNavigation { get; set; } = null!;
}
