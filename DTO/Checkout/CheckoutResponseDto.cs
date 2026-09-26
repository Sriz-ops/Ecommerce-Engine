namespace EcommerceAPI.DTOs.Checkout
{
    public class CheckoutResponseDto
    {
        public decimal Subtotal { get; set; }

        public decimal ShippingCharge { get; set; }

        public decimal Tax { get; set; }

        public decimal TotalAmount { get; set; }

        public List<PriceChangeDto> PriceChanges { get; set; } = new();

        public List<StockIssueDto> StockIssues { get; set; } = new();

        public int? OrderId { get; set; }
    }
}