using EcommerceAPI.Models.Payment;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<int> CreatePaymentAsync(
            OrderPayment payment,
            CancellationToken cancellationToken);
    }
}