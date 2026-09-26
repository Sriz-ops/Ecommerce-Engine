namespace EcommerceAPI.DTOs.Checkout
{
    public class ShippingMethodDto
    {
        public string Name { get; set; } = string.Empty;

        public decimal Charge { get; set; }
    }
}