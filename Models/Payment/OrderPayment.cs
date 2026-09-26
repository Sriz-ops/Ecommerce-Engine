namespace EcommerceAPI.Models.Payment
{
    public class OrderPayment
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public string PaymentStatus { get; set; } = "Pending";

        public string? PaymentMethod { get; set; }

        public decimal Amount { get; set; }

        public string? TransactionReference { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }
    }
}