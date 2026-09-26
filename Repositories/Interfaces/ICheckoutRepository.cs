using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface ICheckoutRepository
    {
        Task<List<CartItem>> GetCartItemsAsync(
            int customerId,
            CancellationToken cancellationToken);
    }
}