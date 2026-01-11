using System;
using System.Collections.Generic;

namespace newltweb.Models;

public partial class THoaDonBan
{
    public int SoHdb { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime NgayBan { get; set; }

    public byte TrangThai { get; set; }

    public decimal TongTien { get; set; }

    public decimal PhiShip { get; set; }

    public decimal GiamGia { get; set; }

    public DateTime Tgtao { get; set; }

    public DateTime TgcapNhat { get; set; }

    public virtual ICollection<TChiTietHdb> TChiTietHdbs { get; set; } = new List<TChiTietHdb>();
}
