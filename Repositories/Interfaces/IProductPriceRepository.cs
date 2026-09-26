using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IProductPriceRepository
    {
        Task<ProductPrice?> GetByIdAsync(int id);

        Task<ProductPrice?> GetCurrentPriceByProductIdAsync(int productId);

        Task<int> AddAsync(ProductPrice productPrice);
    }
}