using EcommerceAPI.DTOs;
using EcommerceAPI.DTOs.Inventory;
using EcommerceAPI.Models;
using EcommerceAPI.Models.Inventory;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync(int customerId, CancellationToken cancellationToken);

        Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

        Task<ProductPagedResponseDto> SearchAsync(ProductQueryDto query, CancellationToken cancellationToken);

        Task<int> CreateAsync(Product product, CancellationToken cancellationToken);

        Task<bool> UpdateAsync(int id, Product product, CancellationToken cancellationToken);

        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);

        Task<Inventory?> UpdateStockAsync(int productId,int quantity,CancellationToken cancellationToken);

        Task<List<Inventory>> BulkUpdateStockAsync(List<BulkInventoryUpdateDto> items,CancellationToken cancellationToken);

        Task<bool> UpdateImagePathAsync(int id, string imagePath, CancellationToken cancellationToken);
    }
}