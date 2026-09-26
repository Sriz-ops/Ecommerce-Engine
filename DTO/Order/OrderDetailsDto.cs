using EcommerceAPI.DTOs.Payment;

namespace EcommerceAPI.DTOs.Order
{
    public class OrderDetailsDto
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingCharge { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();

        public OrderShippingDto? Shipping { get; set; }

        public OrderPaymentDto? Payment { get; set; }
    }
}