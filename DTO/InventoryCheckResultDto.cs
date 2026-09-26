namespace EcommerceAPI.DTO
{
    public class InventoryCheckResultDto
    {
        public int ProductId {  get; set; }

        public int RequestedQuantity {  get; set; }

        public int AvailableQuantity {  get; set; }

        public bool IsAvailable {  get; set; }
    }
}
