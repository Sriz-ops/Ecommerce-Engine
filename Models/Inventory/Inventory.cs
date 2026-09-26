namespace EcommerceAPI.Models.Inventory
{
    public class Inventory
    {
        public int InventoryId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}