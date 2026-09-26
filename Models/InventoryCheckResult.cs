namespace EcommerceAPI.Models
{
    public class InventoryCheckResult
    {
        public int ProductId { get; set; }

        public int RequestedQuantity { get; set; }

        public int AvailableQuantity { get; set; }

        public bool IsAvailable { get; set; }
    }
}