namespace EcommerceAPI.DTO
{
    public class ProductAvailabilityDto
    {
        public int ProductId { get; set; }

        public bool IsAvailable { get; set; }

        public int StockQuantity { get; set;}
    }
}
