namespace EcommerceAPI.DTOs.Checkout
{
    public class PriceChangeDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal OldPrice { get; set; }

        public decimal CurrentPrice { get; set; }
    }
}