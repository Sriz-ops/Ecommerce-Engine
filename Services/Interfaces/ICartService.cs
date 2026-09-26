using EcommerceAPI.DTO;
using EcommerceAPI.DTOs;

namespace EcommerceAPI.Services.Interfaces
{
    public interface ICartService
    {
        Task<List<CartItemDto>> GetCartAsync(int customerId, CancellationToken cancellationToken);

        Task AddItemAsync(
            int customerId,
            AddCartItemDto dto,
            CancellationToken cancellationToken);

        Task<bool> UpdateItemAsync(
            int customerId,
            int productId,
            UpdateCartItemDto dto,
            CancellationToken cancellationToken);

        Task<bool> RemoveItemAsync(
            int customerId,
            int productId,
            CancellationToken cancellationToken);

        Task ClearCartAsync(int customerId,
            CancellationToken cancellationToken);

        Task<int> GetItemCountAsync(int customerId,
            CancellationToken cancellationToken);

        Task<decimal> GetSubtotalAsync(int customerId,
            CancellationToken cancellationToken);
    }
}