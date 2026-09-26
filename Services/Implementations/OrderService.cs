using EcommerceAPI.DTO;
using AutoMapper;
using EcommerceAPI.DTOs.Order;
using EcommerceAPI.Models.Order;
using EcommerceAPI.Repositories.Interfaces;
using EcommerceAPI.Services.Interfaces;

namespace EcommerceAPI.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            ICartService cartService,
            IMapper mapper  )
        {
            _orderRepository = orderRepository;
            _cartService = cartService;
            _mapper = mapper;
        }

        public async Task<Order?> GetByIdAsync(
        int orderId,
        CancellationToken cancellationToken)
        {
            return await _orderRepository.GetByIdAsync(
                orderId,
                cancellationToken);
        }
        public async Task<int> CreateOrderAsync(
            Order order,
            CancellationToken cancellationToken)
        {
            return await _orderRepository.CreateOrderAsync(
                order,
                cancellationToken);
        }

        public async Task<int> CreateOrderItemAsync(
            OrderItem orderItem,
            CancellationToken cancellationToken)
        {
            return await _orderRepository.CreateOrderItemAsync(
                orderItem,
                cancellationToken);
        }

        public async Task<int> CreateOrderShippingAsync(
            OrderShipping orderShipping,
            CancellationToken cancellationToken)
        {
            return await _orderRepository.CreateOrderShippingAsync(
                orderShipping,
                cancellationToken);
        }

        public async Task<int> CreateCompleteOrderAsync(
            Order order,
            List<OrderItem> orderItems,
            OrderShipping shipping,
            CancellationToken cancellationToken)
        {
            if (orderItems == null || orderItems.Count == 0)
            {
                throw new InvalidOperationException(
                    "Order must contain at least one item.");
            }

            var orderId =
                await _orderRepository.CreateOrderAsync(
                    order,
                    cancellationToken);

            foreach (var item in orderItems)
            {
                item.OrderId = orderId;

                await _orderRepository.CreateOrderItemAsync(
                    item,
                    cancellationToken);
            }

            shipping.OrderId = orderId;

            await _orderRepository.CreateOrderShippingAsync(
                shipping,
                cancellationToken);

            return orderId;
        }

        public async Task UpdateOrderStatusAsync(
     int orderId,
     string newStatus,
     int updatedBy,
     CancellationToken cancellationToken)
        {
            var order =
                await _orderRepository.GetByIdAsync(
                    orderId,
                    cancellationToken);

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            var currentStatus = order.OrderStatus;

            var validTransition = currentStatus switch
            {
                "Pending" =>
                    newStatus is "Processing" or "Cancelled",

                "Placed" =>
                    newStatus is "Processing" or "Cancelled",

                "Processing" =>
                    newStatus is "Shipped" or "Cancelled",

                "Shipped" =>
                    newStatus == "Delivered",

                "Delivered" => false,

                "Cancelled" => false,

                _ => false
            };

            if (!validTransition)
            {
                throw new InvalidOperationException(
                    $"Invalid order status transition: " +
                    $"{currentStatus} → {newStatus}");
            }

            await _orderRepository.UpdateOrderStatusAsync(
                orderId,
                newStatus,
                updatedBy,
                cancellationToken);

            await _orderRepository.AddOrderTrackingAsync(
                new OrderTracking
                {
                    OrderId = orderId,
                    Status = newStatus,
                    TrackingNote = $"Order status changed to {newStatus}",
                    CreatedBy = updatedBy
                },
                cancellationToken);
        }

        public async Task<(List<Order> Orders, int TotalRecords)> GetOrderHistoryAsync(
    int customerId,
    string? status,
    int page,
    int pageSize,
    CancellationToken cancellationToken)
        {
            return await _orderRepository.GetOrderHistoryAsync(
                customerId,
                status,
                page,
                pageSize,
                cancellationToken);
        }

        public async Task ReorderAsync(
    int customerId,
    int orderId,
    CancellationToken cancellationToken)
        {
            var order =
                await _orderRepository.GetByIdAsync(
                    orderId,
                    cancellationToken);

            if (order == null)
            {
                throw new InvalidOperationException(
                    "Order not found.");
            }

            if (order.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to reorder this order.");
            }

            var items =
                await _orderRepository.GetOrderItemsAsync(
                    orderId,
                    cancellationToken);

            foreach (var item in items)
            {
                await _cartService.AddItemAsync(
                    customerId,
                    new DTO.AddCartItemDto
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    }, cancellationToken);
            }
        }

        public async Task<OrderDetailsDto?> GetOrderDetailsAsync(
            int orderId,
            CancellationToken cancellationToken)
        {
            var order =
                await _orderRepository.GetByIdAsync(
                    orderId,
                    cancellationToken);

            if (order == null)
            {
                return null;
            }

            var items =
                await _orderRepository.GetOrderItemsAsync(
                    orderId,
                    cancellationToken);

            var shipping =
                await _orderRepository.GetOrderShippingAsync(
                    orderId,
                    cancellationToken);

            var payment =
                await _orderRepository.GetOrderPaymentAsync(
                    orderId,
                    cancellationToken);

            return new OrderDetailsDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                Subtotal = order.Subtotal,
                ShippingCharge = order.ShippingCharge,
                Tax = order.Tax,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                CreatedAt = order.CreatedAt,

                Items = items.Select(item => new OrderItemDto
                {
                    OrderItemId = item.OrderItemId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    ItemSubtotal = item.ItemSubtotal
                }).ToList(),

                Shipping = shipping == null
                    ? null
                    : new OrderShippingDto
                    {
                        ShippingName = shipping.ShippingName,
                        ShippingAddress = shipping.ShippingAddress,
                        ShippingCity = shipping.ShippingCity,
                        ShippingState = shipping.ShippingState,
                        ShippingPostalCode = shipping.ShippingPostalCode,
                        ShippingMethod = shipping.ShippingMethod,
                        ShippingCharge = shipping.ShippingCharge
                    },

                Payment = payment == null
                    ? null
                    : new EcommerceAPI.DTOs.Payment.OrderPaymentDto
                    {
                        PaymentId = payment.PaymentId,
                        OrderId = payment.OrderId,
                        PaymentStatus = payment.PaymentStatus,
                        PaymentMethod = payment.PaymentMethod,
                        Amount = payment.Amount,
                        TransactionReference = payment.TransactionReference,
                        CreatedAt = payment.CreatedAt
                    }
            };
        }
        public async Task<List<OrderTrackingDto>> GetOrderTrackingAsync(
    int orderId,
    int customerId,
    CancellationToken cancellationToken)
        {
            var order =
                await _orderRepository.GetByIdAsync(
                    orderId,
                    cancellationToken);

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            if (order.CustomerId != customerId)
                throw new UnauthorizedAccessException(
                    "You are not authorized to view this order.");

            var tracking =
                await _orderRepository.GetOrderTrackingAsync(
                    orderId,
                    cancellationToken);

            return _mapper.Map<List<OrderTrackingDto>>(tracking);
        }
    }

}