using System;
using System.Collections.Generic;

namespace QuanLyNhaSachAPI.Models;

public partial class NguoiQuanTri
{
    public int MaNd { get; set; }

    public DateOnly? NgayVaoLam { get; set; }

    public virtual NguoiDung MaNdNavigation { get; set; } = null!;
}
