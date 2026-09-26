namespace EcommerceAPI.Models.Order
{
    public class OrderShipping
    {
        public int OrderShippingId { get; set; }

        public int OrderId { get; set; }

        public string ShippingName { get; set; } = string.Empty;

        public string ShippingAddress { get; set; } = string.Empty;

        public string ShippingCity { get; set; } = string.Empty;

        public string ShippingState { get; set; } = string.Empty;

        public string ShippingPostalCode { get; set; } = string.Empty;

        public string ShippingMethod { get; set; } = string.Empty;

        public decimal ShippingCharge { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }
    }
}