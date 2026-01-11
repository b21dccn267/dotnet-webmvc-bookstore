using Microsoft.EntityFrameworkCore;
using newltweb.DTOs.Cart;
using newltweb.Models;
using System;

namespace newltweb.Services
{
    public class OrderService : IOrderService
    {
        private readonly LtwebBtlContext _db;
        private readonly ICartService _cartService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(LtwebBtlContext db, ICartService cartService, ILogger<OrderService> logger)
        {
            _db = db;
            _cartService = cartService;
            _logger = logger;
        }

        public async Task<OrderResult> CreateOrderFromCartAsync(string userId)
        {
            var validation = await _cartService.ValidateCartForCheckoutAsync(userId);
            if (!validation.IsValid)
                return OrderResult.Failed(validation.ErrorMessage ?? "Cart validation failed");

            var snapshot = await _cartService.GetCartSnapshotAsync(userId);
            if (snapshot == null || snapshot.Items == null)
                return OrderResult.Failed("Giỏ hàng trống");

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in snapshot.Items)
                {
                    var productEntity = await _db.TSaches
                        .Where(p => p.MaSach == item.MaSach)
                        .SingleOrDefaultAsync();

                    if (productEntity == null)
                    {
                        return OrderResult.Failed($"Sản phẩm không tồn tại: {item.TenSach}");
                    }
                        
                    if (productEntity.SoLuong < item.SoLuong)
                    {
                        return OrderResult.Failed($"Sản phẩm {item.TenSach} chỉ còn {productEntity.SoLuong} sản phẩm");
                    }

                    if (productEntity.DonGiaBan != item.DonGiaBan)
                    {
                        return OrderResult.Failed($"Giá của {item.TenSach} đã thay đổi");
                    }  
                }

                // create order header
                var order = new THoaDonBan
                {
                    UserId = userId,
                    NgayBan = DateTime.UtcNow,
                    TrangThai = 1,
                    TongTien = snapshot.Tong,
                    PhiShip = snapshot.PhiShip,
                    GiamGia = snapshot.GiamGia,
                };

                _db.THoaDonBans.Add(order);
                await _db.SaveChangesAsync();

                foreach (var item in snapshot.Items)
                {
                    var productEntity = await _db.TSaches
                        .Where(p => p.MaSach == item.MaSach)
                        .SingleAsync();

                    var orderItem = new TChiTietHdb
                    {
                        SoHdb = order.SoHdb,
                        MaSach = item.MaSach,
                        TenSach = item.TenSach,
                        SoLuong = item.SoLuong,
                        DonGiaBan = item.DonGiaBan,
                        TongTien = item.TongTien
                    };
                    _db.TChiTietHdbs.Add(orderItem);

                    productEntity.SoLuong -= item.SoLuong;
                    _db.TSaches.Update(productEntity);
                }

                await _db.SaveChangesAsync();

                await _cartService.ClearCartAsync(userId);

                await tx.CommitAsync();

                return OrderResult.Ok(order.SoHdb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateOrderFromCart failed for user {UserId}", userId);
                try { await tx.RollbackAsync(); } catch { /* ignore rollback failure */ }
                return OrderResult.Failed("Lỗi khi tạo đơn hàng. Vui lòng thử lại.");
            }
        }
    }

}
