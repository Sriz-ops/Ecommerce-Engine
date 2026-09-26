using EcommerceAPI.DTOs.Payment;
using EcommerceAPI.Models.Payment;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services.Implementations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        public async Task<MockPaymentResponseDto> ProcessPaymentAsync(
    int customerId,
    MockPaymentRequestDto request,
    CancellationToken cancellationToken)
        {
            var order =
                await _orderRepository.GetByIdAsync(
                    request.OrderId,
                    cancellationToken);

            if (order == null)
            {
                throw new InvalidOperationException(
                    "Order not found.");
            }

            if (order.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to pay for this order.");
            }

            var transactionReference =
                Guid.NewGuid().ToString();

            var payment = new OrderPayment
            {
                OrderId = request.OrderId,
                PaymentStatus = "Success",
                PaymentMethod = request.PaymentMethod,
                Amount = order.TotalAmount,
                TransactionReference = transactionReference,
                CreatedBy = customerId
            };

            await _paymentRepository.CreatePaymentAsync(
                payment,
                cancellationToken);

            // Update order status after successful payment
            await _orderRepository.UpdateOrderStatusAsync(
                request.OrderId,
                "Placed",
                customerId,
                cancellationToken);

            return new MockPaymentResponseDto
            {
                OrderId = request.OrderId,
                PaymentStatus = payment.PaymentStatus,
                TransactionReference = transactionReference,
                Amount = payment.Amount
            };
        }
    }
}