using EcommerceAPI.DTOs.Payment;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<MockPaymentResponseDto> ProcessPaymentAsync(
            int customerId,
            MockPaymentRequestDto request,
            CancellationToken cancellationToken);
    }
}