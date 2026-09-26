using System.ComponentModel.DataAnnotations;

namespace EcommerceAPI.DTOs.Inventory
{
    public class BulkInventoryUpdateDto
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}