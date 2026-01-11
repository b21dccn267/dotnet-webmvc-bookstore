using System;
using System.Collections.Generic;

namespace newltweb.Models;

public partial class TSach
{
    public string MaSach { get; set; } = null!;

    public string? TenSach { get; set; }

    public string? TacGia { get; set; }

    public string? MaTheLoai { get; set; }

    public string? MaNxb { get; set; }

    public decimal? DonGiaNhap { get; set; }

    public decimal? DonGiaBan { get; set; }

    public int? SoLuong { get; set; }

    public int? SoTrang { get; set; }

    public string? Anh { get; set; }

    public virtual TNhaXuatBan? MaNxbNavigation { get; set; }

    public virtual TTheLoai? MaTheLoaiNavigation { get; set; }

    public virtual ICollection<TChiTietHdb> TChiTietHdbs { get; set; } = new List<TChiTietHdb>();

    public virtual ICollection<TChiTietHdn> TChiTietHdns { get; set; } = new List<TChiTietHdn>();

    public virtual ICollection<TSachGh> TSachGhs { get; set; } = new List<TSachGh>();
}
