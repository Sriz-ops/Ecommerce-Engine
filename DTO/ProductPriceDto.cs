namespace EcommerceAPI.DTOs
{
    public class ProductPriceDto
    {
        public int ProductPriceId { get; set; }

        public int ProductId { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}