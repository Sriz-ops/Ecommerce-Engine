using System.Text.Json.Serialization;

namespace EcommerceAPI.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int BrandId { get; set; }

        public string BrandName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal Rating { get; set; }

        [JsonIgnore]
        public string? ImagePath { get; set; }
    }
}