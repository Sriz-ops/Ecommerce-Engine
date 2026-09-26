using EcommerceAPI.DTOs.Address;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IAddressService
    {
        Task<int> CreateAsync(
            AddressDto dto,
            int customerId,
            CancellationToken cancellationToken);

        Task<List<AddressDto>> GetAllAsync(
            int customerId,
            CancellationToken cancellationToken);

        Task<bool> UpdateAsync(
            int addressId,
            AddressDto dto,
            int customerId,
            CancellationToken cancellationToken);

        Task<bool> DeleteAsync(
            int addressId,
            int customerId,
            CancellationToken cancellationToken);
    }
}