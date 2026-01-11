namespace newltweb.DTOs.Cart
{
    public class CheckoutValidationResult
    {
        public bool IsValid { get; init; }
        public string? ErrorMessage { get; init; }
    }
}
