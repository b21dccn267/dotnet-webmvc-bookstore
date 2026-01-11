namespace newltweb.DTOs.Cart
{
    public class OrderResult
    {
        public bool Success { get; init; }
        public int? OrderId { get; init; }
        public string? ErrorMessage { get; init; }

        public static OrderResult Failed(string msg) => new OrderResult { Success = false, ErrorMessage = msg };
        public static OrderResult Ok(int orderId) => new OrderResult { Success = true, OrderId = orderId };
    }
}
