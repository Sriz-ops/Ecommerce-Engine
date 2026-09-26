namespace EcommerceAPI.DTOs.Order
{
    public class OrderDto
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public decimal Subtotal { get; set; }

        public decimal ShippingCharge { get; set; }

        public decimal Tax { get; set; }

        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}