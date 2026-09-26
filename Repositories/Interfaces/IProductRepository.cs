using EcommerceAPI.DTOs;
using EcommerceAPI.DTOs.Inventory;
using EcommerceAPI.Models;
using EcommerceAPI.Models.Inventory;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<List<ProductDetails>> GetAllAsync(CancellationToken cancellationToken);

        Task<ProductDetails?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<ProductPagedResult> SearchAsync(
            string? search,
            int? categoryId,
            int? brandId,
            decimal? minPrice,
            decimal? maxPrice,
            decimal? minRating,
            string? sortBy,
            string? sortOrder,
            int page,
            int pageSize,
            CancellationToken cancellationToken);

        Task<int> CreateAsync(Product product, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(int id, Product product, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

        Task<bool> UpdateImagePathAsync(
        int id,
        string imagePath,
        CancellationToken cancellationToken);

        Task<Inventory?> UpdateStockAsync(
        int productId,
        int quantity,
        CancellationToken cancellationToken);

        Task<List<Inventory>> BulkUpdateStockAsync(
        List<BulkInventoryUpdateDto> items,
        CancellationToken cancellationToken);

    }
}