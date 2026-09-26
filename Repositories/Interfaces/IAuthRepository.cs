using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<Customer?> GetByEmailAsync(string email);

        Task<Customer?> GetByIdAsync(int customerId);

        Task<int> CreateCustomerAsync(Customer customer);
    }
}