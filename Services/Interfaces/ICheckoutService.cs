using EcommerceAPI.DTOs.Checkout;

namespace EcommerceAPI.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutResponseDto> CalculateCheckoutAsync(
            int customerId,
            string CustomerName,
            CheckoutRequestDto request,
            CancellationToken cancellationToken);
    }
}