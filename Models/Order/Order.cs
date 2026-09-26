namespace EcommerceAPI.Models.Order
{
    public class Order
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public decimal Subtotal { get; set; }

        public decimal ShippingCharge { get; set; }

        public decimal Tax { get; set; }

        public decimal TotalAmount { get; set; }

        public string OrderStatus { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }
    }
}