using newltweb.DTOs.Cart;

namespace newltweb.Services
{
    public interface IOrderService
    {
        Task<OrderResult> CreateOrderFromCartAsync(string userId);
    }
}
