using EcommerceAPI.DTOs.Address;
using EcommerceAPI.Models.Address;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services.Implementations
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;

        public AddressService(
            IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        public async Task<int> CreateAsync(
            AddressDto dto,
            int customerId,
            CancellationToken cancellationToken)
        {
            var address = new Address
            {
                CustomerId = customerId,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                IsDefault = dto.IsDefault
            };

            return await _addressRepository.CreateAsync(
                address,
                cancellationToken);
        }

        public async Task<List<AddressDto>> GetAllAsync(
            int customerId,
            CancellationToken cancellationToken)
        {
            var addresses =
                await _addressRepository.GetByCustomerIdAsync(
                    customerId,
                    cancellationToken);

            return addresses.Select(x => new AddressDto
            {
                AddressId = x.AddressId,
                CustomerId = x.CustomerId,
                AddressLine1 = x.AddressLine1,
                AddressLine2 = x.AddressLine2,
                City = x.City,
                State = x.State,
                PostalCode = x.PostalCode,
                Country = x.Country,
                IsDefault = x.IsDefault
            }).ToList();
        }

        public async Task<bool> UpdateAsync(
            int addressId,
            AddressDto dto,
            int customerId,
            CancellationToken cancellationToken)
        {
            var address = new Address
            {
                AddressId = addressId,
                CustomerId = customerId,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                IsDefault = dto.IsDefault
            };

            return await _addressRepository.UpdateAsync(
                address,
                cancellationToken);
        }

        public async Task<bool> DeleteAsync(
            int addressId,
            int customerId,
            CancellationToken cancellationToken)
        {
            return await _addressRepository.DeleteAsync(
                addressId,
                customerId,
                cancellationToken);
        }
    }
}