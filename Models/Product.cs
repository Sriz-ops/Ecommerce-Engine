using System.ComponentModel.DataAnnotations;
namespace EcommerceAPI.Models
{
    public class Product
    {
        public int ProductId {  get; set; }
        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Range(0, 5)]
        public decimal Rating { get; set; }

        public string? ImagePath { get; set; }  
    }
}
