using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class ProductPrice
    {
        public int ProductPriceId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}