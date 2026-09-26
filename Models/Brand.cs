using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.Models
{
    public class Brand
    {
        public int BrandId{get; set;}

        [Required]
        public string Name{get; set;} = string.Empty;
    }
}
