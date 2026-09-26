using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IProductAvailabilityRepository
    {
        Task<ProductAvailability?> GetAvailabilityAsync(int productId, CancellationToken cancellationToken);

        Task<List<InventoryCheckResult>> CheckInventoryAsync(
            List<InventoryCheckItem> items, CancellationToken cancellationToken);
    }
}