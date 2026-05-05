using System;
using System.Collections.Generic;

namespace QuanLyNhaSachAPI.Models;

public partial class TacGium
{
    public int MaTg { get; set; }

    public string TenTg { get; set; } = null!;

    public virtual ICollection<Sach> Saches { get; set; } = new List<Sach>();
}
