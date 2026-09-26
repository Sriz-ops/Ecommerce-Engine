namespace EcommerceAPI.DTOs.Payment
{
    public class MockPaymentResponseDto
    {
        public int OrderId { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public string TransactionReference { get; set; } = string.Empty;

        public decimal Amount { get; set; }
    }
}