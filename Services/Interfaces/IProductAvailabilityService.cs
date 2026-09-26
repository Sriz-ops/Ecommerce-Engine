using EcommerceAPI.DTO;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IProductAvailabilityService
    {
        Task<ProductAvailabilityDto?> GetAvailabilityAsync(
            int productId, CancellationToken cancellationToken);

        Task<List<InventoryCheckResultDto>> CheckInventoryAsync(
            List<InventoryCheckItemDto> items, CancellationToken cancellationToken);
    }
}