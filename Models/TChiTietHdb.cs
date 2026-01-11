using System;
using System.Collections.Generic;

namespace newltweb.Models;

public partial class TChiTietHdb
{
    public int Id { get; set; }

    public int SoHdb { get; set; }

    public string? MaSach { get; set; }

    public string TenSach { get; set; } = null!;

    public int SoLuong { get; set; }

    public decimal DonGiaBan { get; set; }

    public decimal TongTien { get; set; }

    public DateTime Tgtao { get; set; }

    public virtual TSach? MaSachNavigation { get; set; }

    public virtual THoaDonBan SoHdbNavigation { get; set; } = null!;
}
