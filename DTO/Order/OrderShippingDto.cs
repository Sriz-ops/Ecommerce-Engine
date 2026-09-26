namespace EcommerceAPI.DTOs.Order
{
    public class OrderShippingDto
    {
        public string ShippingName { get; set; } = string.Empty;

        public string ShippingAddress { get; set; } = string.Empty;

        public string ShippingCity { get; set; } = string.Empty;

        public string ShippingState { get; set; } = string.Empty;

        public string ShippingPostalCode { get; set; } = string.Empty;

        public string ShippingMethod { get; set; } = string.Empty;

        public decimal ShippingCharge { get; set; }
    }
}