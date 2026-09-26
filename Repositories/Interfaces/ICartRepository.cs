using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<List<CartItem>> GetCartAsync(int customerId, CancellationToken cancellationToken);

        Task AddItemAsync(int customerId,int productId,int quantity, CancellationToken cancellationToken);

        Task<bool> UpdateItemAsync(int customerId,int productId,int quantity, CancellationToken cancellationToken);

        Task<bool> RemoveItemAsync(int customerId,int productId, CancellationToken cancellationToken);

        Task ClearCartAsync(int customerId, CancellationToken cancellationToken);

        Task<int> GetItemCountAsync(int customerId, CancellationToken cancellationToken);

        Task<decimal> GetSubtotalAsync(int customerId, CancellationToken cancellationToken);
    }
}