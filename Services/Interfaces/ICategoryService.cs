using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();

        Task<CategoryDto?> GetByIdAsync(int id);
    }
}