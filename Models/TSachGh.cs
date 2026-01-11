using System;
using System.Collections.Generic;

namespace newltweb.Models;

public partial class TSachGh
{
    public int Id { get; set; }

    public int MaGh { get; set; }

    public string MaSach { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal DonGiaBan { get; set; }

    public DateTime Tgthem { get; set; }

    public virtual TGioHang MaGhNavigation { get; set; } = null!;

    public virtual TSach MaSachNavigation { get; set; } = null!;
}
