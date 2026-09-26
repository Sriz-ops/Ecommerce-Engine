using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.DTOs.Inventory
{
    public class UpdateStockDto
    {
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}