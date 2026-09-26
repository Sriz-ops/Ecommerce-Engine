using System.Security.Claims;
using Asp.Versioning;
using EcommerceAPI.DTOs.Address;
using EcommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/addresses")]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Customer")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(
            IAddressService addressService)
        {
            _addressService = addressService;
        }

        private int GetCustomerId()
        {
            return int.Parse(
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)!);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            AddressDto dto,
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            var addressId =
                await _addressService.CreateAsync(
                    dto,
                    customerId,
                    cancellationToken);

            return Ok(new
            {
                AddressId = addressId,
                Message = "Address created successfully."
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            var addresses =
                await _addressService.GetAllAsync(
                    customerId,
                    cancellationToken);

            return Ok(addresses);
        }

        [HttpPut("{addressId}")]
        public async Task<IActionResult> Update(
            int addressId,
            AddressDto dto,
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            var updated =
                await _addressService.UpdateAsync(
                    addressId,
                    dto,
                    customerId,
                    cancellationToken);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{addressId}")]
        public async Task<IActionResult> Delete(
            int addressId,
            CancellationToken cancellationToken)
        {
            var customerId = GetCustomerId();

            var deleted =
                await _addressService.DeleteAsync(
                    addressId,
                    customerId,
                    cancellationToken);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}