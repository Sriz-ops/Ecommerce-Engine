using EcommerceAPI.Models.Order;
using EcommerceAPI.Models.Payment;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<int> CreateOrderItemAsync(
         OrderItem orderItem,
         CancellationToken cancellationToken);

        Task<int> CreateOrderShippingAsync(
            OrderShipping orderShipping,
            CancellationToken cancellationToken);

        Task<int> CreateOrderAsync(
            Order order,
            CancellationToken cancellationToken);

        Task<Order?> GetByIdAsync(
            int orderId,
            CancellationToken cancellationToken);

        Task<List<OrderItem>> GetOrderItemsAsync(
            int orderId,
            CancellationToken cancellationToken);

        Task<OrderShipping?> GetOrderShippingAsync(
            int orderId,
            CancellationToken cancellationToken);

        Task<OrderPayment?> GetOrderPaymentAsync(
            int orderId,
            CancellationToken cancellationToken);

        Task UpdateOrderStatusAsync(
            int orderId,
            string orderStatus,
            int updatedBy,
            CancellationToken cancellationToken);
        Task<int> AddOrderTrackingAsync(
             OrderTracking tracking,
            CancellationToken cancellationToken);

        Task<List<OrderTracking>> GetOrderTrackingAsync(
            int orderId,
            CancellationToken cancellationToken);

        Task<(List<Order> Orders, int TotalRecords)> GetOrderHistoryAsync(
        int customerId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
    }
}