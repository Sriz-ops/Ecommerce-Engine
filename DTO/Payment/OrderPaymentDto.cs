namespace EcommerceAPI.DTOs.Payment
{
    public class OrderPaymentDto
    {
        public int PaymentId { get; set; }

        public int OrderId { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public string? PaymentMethod { get; set; }

        public decimal Amount { get; set; }

        public string? TransactionReference { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}