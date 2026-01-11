using System.Collections.Generic;

namespace newltweb.DTOs.Cart
{
    public class CartSnapshotItem
    {
        public int MaSachGH { get; set; }
        public string MaSach { get; set; } = null!;
        public string TenSach { get; set; } = null!;
        public string TacGia { get; set; } = null!;
        public decimal DonGiaBan { get; set; }
        public int SoLuong { get; set; }
        public string? Anh { get; set; }
        public decimal TongTien { get; set; }
    }
}
