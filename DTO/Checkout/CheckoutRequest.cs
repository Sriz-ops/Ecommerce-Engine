namespace EcommerceAPI.DTOs.Checkout
{
    public class CheckoutRequestDto
    {
        public int AddressId { get; set; }

        public string ShippingMethod { get; set; } = string.Empty;

        public string? PaymentMethod { get; set; }
    }
}