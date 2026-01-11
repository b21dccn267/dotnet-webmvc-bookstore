using newltweb.DTOs.Cart;
using newltweb.Models;
using System.Threading;
using System.Threading.Tasks;

namespace newltweb.Services
{
    public interface ICartService
    {
        Task<TGioHang> GetOrCreateCartAsync(string userId, CancellationToken ct = default);
        Task<TGioHang> GetCartForUserAsync(string userId, CancellationToken ct = default);
        Task AddToCartAsync(string userId, string maSach, int soLuong = 1, CancellationToken ct = default);
        Task UpdateQuantityAsync(string userId, int maSachGH, int soLuong, CancellationToken ct = default);
        Task RemoveItemAsync(string userId, int maSachGH, CancellationToken ct = default);
        Task ClearCartAsync(string userId, CancellationToken ct = default);
        Task<CartSnapshot> GetCartSnapshotAsync(string userId, CancellationToken ct = default);
        Task<int> GetCartItemCountAsync(string userId, CancellationToken ct = default);
        Task<CheckoutValidationResult> ValidateCartForCheckoutAsync(string userId);
    }
}