using EcommerceAPI.Models.Address;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IAddressRepository
    {
        Task<int> CreateAsync(
            Address address,
            CancellationToken cancellationToken);

        Task<List<Address>> GetByCustomerIdAsync(
            int customerId,
            CancellationToken cancellationToken);

        Task<Address?> GetByIdAsync(
            int addressId,
            int customerId,
            CancellationToken cancellationToken);

        Task<bool> UpdateAsync(
            Address address,
            CancellationToken cancellationToken);

        Task<bool> DeleteAsync(
            int addressId,
            int customerId,
            CancellationToken cancellationToken);
    }
}