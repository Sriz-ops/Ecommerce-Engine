using EcommerceAPI.DTOs.Order;
using EcommerceAPI.Models.Order;

namespace EcommerceAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> CreateOrderAsync(
            Order order,
            CancellationToken cancellationToken);

        Task<int> CreateOrderItemAsync(
            OrderItem orderItem,
            CancellationToken cancellationToken);

        Task<int> CreateOrderShippingAsync(
            OrderShipping orderShipping,
            CancellationToken cancellationToken);

        Task<int> CreateCompleteOrderAsync(
            Order order,
            List<OrderItem> orderItems,
            OrderShipping shipping,
            CancellationToken cancellationToken);

        Task UpdateOrderStatusAsync(
             int orderId,
            string orderStatus,
             int updatedBy,
             CancellationToken cancellationToken);
        Task<Order?> GetByIdAsync(
            int orderId,
            CancellationToken cancellationToken);

        Task<OrderDetailsDto?> GetOrderDetailsAsync(
            int orderId,
            CancellationToken cancellationToken);

        Task<(List<Order> Orders, int TotalRecords)> GetOrderHistoryAsync(
           int customerId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

        Task ReorderAsync(
            int customerId,
            int orderId,
            CancellationToken cancellationToken);

        Task<List<OrderTrackingDto>> GetOrderTrackingAsync(
        int orderId,
        int customerId,
        CancellationToken cancellationToken);
    }
}