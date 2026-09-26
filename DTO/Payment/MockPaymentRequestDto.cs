namespace EcommerceAPI.DTOs.Payment
{
    public class MockPaymentRequestDto
    {
        public int OrderId { get; set; }

        public string? PaymentMethod { get; set; }
    }
}