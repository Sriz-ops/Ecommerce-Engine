using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IProductPriceService
    {
        Task<ProductPriceDto?> GetByIdAsync(int id);

        Task<ProductPriceDto?> GetCurrentPriceByProductIdAsync(int productId);

        Task<int> AddAsync(CreateProductPriceDto dto);
    }
}