namespace EcommerceAPI.DTOs.Checkout
{
    public class StockIssueDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int RequestedQuantity { get; set; }

        public int AvailableQuantity { get; set; }
    }
}