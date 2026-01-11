using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using newltweb.DTOs.Cart;
using newltweb.Models;
using newltweb.Services;

namespace newltweb.Services
{
    public class EfCartService : ICartService
    {
        private readonly LtwebBtlContext _db;

        public EfCartService(LtwebBtlContext db)
        {
            _db = db;
        }

        public async Task<TGioHang> GetOrCreateCartAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

            var cart = await _db.TGioHangs
                .Include(gh => gh.TSachGhs)
                .SingleOrDefaultAsync(gh => gh.UserId == userId, ct);

            if (cart != null) return cart;

            cart = new TGioHang
            {
                UserId = userId,
                Tgtao = DateTime.Now,
                TgcapNhat = DateTime.Now
            };
            _db.TGioHangs.Add(cart);
            await _db.SaveChangesAsync(ct);
            return cart;
        }

        public async Task<TGioHang> GetCartForUserAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
            return await _db.TGioHangs
                .Include(gh => gh.TSachGhs)
                .ThenInclude(sgh => sgh.MaSachNavigation)
                .SingleOrDefaultAsync(gh => gh.UserId == userId, ct)
                ?? new TGioHang { UserId = userId, TSachGhs = new System.Collections.Generic.List<TSachGh>() };
        }

        public async Task AddToCartAsync(string userId, string maSach, int soLuong = 1, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
            if (string.IsNullOrWhiteSpace(maSach)) throw new ArgumentNullException(nameof(maSach));
            if (soLuong <= 0) throw new ArgumentException("Số lượng phải lớn hơn 0", nameof(soLuong));

            var book = await _db.TSaches.FindAsync(new object[] { maSach }, ct);
            if (book == null) throw new InvalidOperationException("Không tìm thấy sách");

            // Use transaction to avoid races
            using var tx = await _db.Database.BeginTransactionAsync(ct);

            var cart = await _db.TGioHangs
                .Include(gh => gh.TSachGhs)
                .SingleOrDefaultAsync(gh => gh.UserId == userId, ct);

            if (cart == null)
            {
                cart = new TGioHang
                {
                    UserId = userId,
                    Tgtao = DateTime.Now,
                    TgcapNhat = DateTime.Now
                };
                _db.TGioHangs.Add(cart);
                await _db.SaveChangesAsync(ct);
            }

            var existing = cart.TSachGhs.FirstOrDefault(sgh => sgh.MaSach == maSach);

            if (existing == null)
            {
                var item = new TSachGh
                {
                    MaGh = cart.MaGh,
                    MaSach = maSach,
                    SoLuong = soLuong,
                    DonGiaBan = (decimal)book.DonGiaBan,
                    Tgthem = DateTime.Now
                };
                _db.TSachGhs.Add(item);
            }
            else
            {
                existing.SoLuong += soLuong;
                _db.TSachGhs.Update(existing);
            }

            cart.TgcapNhat = DateTime.Now;
            _db.TGioHangs.Update(cart);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        public async Task UpdateQuantityAsync(string userId, int maSachGH, int soLuong, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));
            if (soLuong < 0) throw new ArgumentException("Số lượng không thể âm", nameof(soLuong));

            var item = await _db.TSachGhs
                .Include(sgh => sgh.MaGhNavigation)
                .SingleOrDefaultAsync(sgh => sgh.Id == maSachGH && sgh.MaGhNavigation.UserId == userId, ct);

            if (item == null) throw new KeyNotFoundException("Không tìm thấy mặt hàng trong giỏ");

            if (soLuong == 0)
            {
                _db.TSachGhs.Remove(item);
            }
            else
            {
                item.SoLuong = soLuong;
                _db.TSachGhs.Update(item);
            }

            item.MaGhNavigation.TgcapNhat = DateTime.Now;
            await _db.SaveChangesAsync(ct);
        }

        public async Task RemoveItemAsync(string userId, int maSachGH, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

            var item = await _db.TSachGhs
                .Include(sgh => sgh.MaGhNavigation)
                .SingleOrDefaultAsync(sgh => sgh.Id == maSachGH && sgh.MaGhNavigation.UserId == userId, ct);

            if (item == null) return;

            _db.TSachGhs.Remove(item);
            item.MaGhNavigation.TgcapNhat = DateTime.Now;
            await _db.SaveChangesAsync(ct);
        }

        public async Task ClearCartAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

            var cart = await _db.TGioHangs
                .Include(gh => gh.TSachGhs)
                .SingleOrDefaultAsync(gh => gh.UserId == userId, ct);

            if (cart == null) return;

            _db.TSachGhs.RemoveRange(cart.TSachGhs);
            cart.TgcapNhat = DateTime.Now;
            await _db.SaveChangesAsync(ct);
        }

        public async Task<CartSnapshot> GetCartSnapshotAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

            var cart = await _db.TGioHangs
                .Include(gh => gh.TSachGhs)
                .ThenInclude(sgh => sgh.MaSachNavigation)
                .SingleOrDefaultAsync(gh => gh.UserId == userId, ct);

            if (cart == null) return new CartSnapshot();

            var items = cart.TSachGhs.Select(sgh => new CartSnapshotItem
            {
                MaSachGH = sgh.Id,
                MaSach = sgh.MaSach,
                TenSach = sgh.MaSachNavigation?.TenSach ?? string.Empty,
                TacGia = sgh.MaSachNavigation?.TacGia ?? string.Empty,
                DonGiaBan = sgh.DonGiaBan,
                SoLuong = sgh.SoLuong,
                Anh = sgh.MaSachNavigation?.Anh,
                TongTien = sgh.DonGiaBan * sgh.SoLuong
            }).ToList();

            var tongGT = items.Sum(sgh => sgh.TongTien);

            return new CartSnapshot
            {
                MaGh = cart.MaGh,
                Items = items,
                TongGTSach = tongGT,
                PhiShip = 0m,
                GiamGia = 0m
            };
        }

        public async Task<int> GetCartItemCountAsync(string userId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentNullException(nameof(userId));

            var count = await _db.TSachGhs
                .Where(sgh => sgh.MaGhNavigation.UserId == userId)
                .SumAsync(sgh => (int?)sgh.SoLuong, ct) ?? 0;

            return count;
        }

        public async Task<CheckoutValidationResult> ValidateCartForCheckoutAsync(string userId)
        {
            var snapshot = await GetCartSnapshotAsync(userId);
            if (snapshot == null || snapshot.Items == null)
                return new CheckoutValidationResult { IsValid = false, ErrorMessage = "Giỏ hàng trống" };

            var bookIds = snapshot.Items.Select(i => i.MaSach).Distinct().ToList();

            var books = await _db.TSaches
                .Where(b => bookIds.Contains(b.MaSach))
                .ToDictionaryAsync(b => b.MaSach);

            foreach (var item in snapshot.Items)
            {
                if (!books.TryGetValue(item.MaSach, out var productEntity))
                {
                    return new CheckoutValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Sản phẩm không tồn tại: {item.TenSach}"
                    };
                }

                if (productEntity.SoLuong < item.SoLuong)
                {
                    return new CheckoutValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Sản phẩm {productEntity.TenSach} chỉ còn {productEntity.SoLuong} sản phẩm"
                    };
                }

                if (productEntity.DonGiaBan != item.DonGiaBan)
                {
                    return new CheckoutValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Giá của {productEntity.TenSach} đã thay đổi từ {item.DonGiaBan:C} thành {productEntity.DonGiaBan:C}"
                    };
                }

                if (item.SoLuong < 1)
                {
                    return new CheckoutValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Số lượng không hợp lệ cho {productEntity.TenSach}"
                    };
                }
            }

            if (snapshot.TongGTSach <= 0)
            {
                return new CheckoutValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Tổng tiền không hợp lệ"
                };
            }

            return new CheckoutValidationResult { IsValid = true };
        }
    }

}