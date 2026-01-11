using System.Collections.Generic;

namespace newltweb.DTOs.Cart
{
    public class CartSnapshot
    {
        public int MaGh { get; set; }
        public List<CartSnapshotItem> Items { get; set; } = new();
        public decimal TongGTSach { get; set; }
        public decimal PhiShip { get; set; }
        public decimal GiamGia { get; set; }
        public decimal Tong => TongGTSach + PhiShip - GiamGia;
    }
}