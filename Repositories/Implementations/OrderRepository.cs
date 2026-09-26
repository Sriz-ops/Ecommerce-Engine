using EcommerceAPI.Models.Order;
using EcommerceAPI.Models.Payment;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EcommerceAPI.Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured.");
        }

        public async Task<int> CreateOrderItemAsync(
            OrderItem orderItem,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_CreateOrderItem",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@OrderId",
                orderItem.OrderId);

            command.Parameters.AddWithValue(
                "@ProductId",
                orderItem.ProductId);

            command.Parameters.AddWithValue(
                "@ProductName",
                orderItem.ProductName);

            command.Parameters.AddWithValue(
                "@Quantity",
                orderItem.Quantity);

            command.Parameters.AddWithValue(
                "@Price",
                orderItem.Price);

            command.Parameters.AddWithValue(
                "@ItemSubtotal",
                orderItem.ItemSubtotal);

            command.Parameters.AddWithValue(
                "@CreatedBy",
                orderItem.CreatedBy);

            await connection.OpenAsync(cancellationToken);

            var result =
                await command.ExecuteScalarAsync(
                    cancellationToken);

            return Convert.ToInt32(result);
        }

        public async Task<int> CreateOrderShippingAsync(
            OrderShipping orderShipping,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_CreateOrderShipping",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@OrderId",
                orderShipping.OrderId);

            command.Parameters.AddWithValue(
                "@ShippingName",
                orderShipping.ShippingName);

            command.Parameters.AddWithValue(
                "@ShippingAddress",
                orderShipping.ShippingAddress);

            command.Parameters.AddWithValue(
                "@ShippingCity",
                orderShipping.ShippingCity);

            command.Parameters.AddWithValue(
                "@ShippingState",
                orderShipping.ShippingState);

            command.Parameters.AddWithValue(
                "@ShippingPostalCode",
                orderShipping.ShippingPostalCode);

            command.Parameters.AddWithValue(
                "@ShippingMethod",
                orderShipping.ShippingMethod);

            command.Parameters.AddWithValue(
                "@ShippingCharge",
                orderShipping.ShippingCharge);

            command.Parameters.AddWithValue(
                "@CreatedBy",
                orderShipping.CreatedBy);

            await connection.OpenAsync(cancellationToken);

            var result =
                await command.ExecuteScalarAsync(
                    cancellationToken);

            return Convert.ToInt32(result);
        }

        public async Task<int> CreateOrderAsync(
            Order order,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_CreateOrder",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@CustomerId",
                order.CustomerId);

            command.Parameters.AddWithValue(
                "@Subtotal",
                order.Subtotal);

            command.Parameters.AddWithValue(
                "@ShippingCharge",
                order.ShippingCharge);

            command.Parameters.AddWithValue(
                "@Tax",
                order.Tax);

            command.Parameters.AddWithValue(
                "@TotalAmount",
                order.TotalAmount);

            command.Parameters.AddWithValue(
                "@OrderStatus",
                order.OrderStatus);

            command.Parameters.AddWithValue(
                "@CreatedBy",
                order.CreatedBy);

            await connection.OpenAsync(cancellationToken);

            var result =
                await command.ExecuteScalarAsync(
                    cancellationToken);

            return Convert.ToInt32(result);
        }

        public async Task<Order?> GetByIdAsync(
            int orderId,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_GetOrderById",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@OrderId",
                orderId);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken);

            if (await reader.ReadAsync(
                cancellationToken))
            {
                return new Order
                {
                    OrderId = reader.GetInt32(
                        reader.GetOrdinal("OrderId")),

                    CustomerId = reader.GetInt32(
                        reader.GetOrdinal("CustomerId")),

                    Subtotal = reader.GetDecimal(
                        reader.GetOrdinal("Subtotal")),

                    ShippingCharge = reader.GetDecimal(
                        reader.GetOrdinal("ShippingCharge")),

                    Tax = reader.GetDecimal(
                        reader.GetOrdinal("Tax")),

                    TotalAmount = reader.GetDecimal(
                        reader.GetOrdinal("TotalAmount")),

                    OrderStatus = reader.GetString(
                        reader.GetOrdinal("OrderStatus")),

                    CreatedAt = reader.GetDateTime(
                        reader.GetOrdinal("CreatedAt")),

                    CreatedBy = reader.GetInt32(
                        reader.GetOrdinal("CreatedBy")),

                    UpdatedAt = reader.IsDBNull(
                        reader.GetOrdinal("UpdatedAt"))
                        ? null
                        : reader.GetDateTime(
                            reader.GetOrdinal("UpdatedAt")),

                    UpdatedBy = reader.IsDBNull(
                        reader.GetOrdinal("UpdatedBy"))
                        ? null
                        : reader.GetInt32(
                            reader.GetOrdinal("UpdatedBy"))
                };
            }

            return null;
        }

        public async Task<List<OrderItem>> GetOrderItemsAsync(
            int orderId,
            CancellationToken cancellationToken)
        {
            var items = new List<OrderItem>();

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_GetOrderItems",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@OrderId",
                orderId);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken);

            while (await reader.ReadAsync(
                cancellationToken))
            {
                items.Add(new OrderItem
                {
                    OrderItemId = reader.GetInt32(
                        reader.GetOrdinal("OrderItemId")),

                    OrderId = reader.GetInt32(
                        reader.GetOrdinal("OrderId")),

                    ProductId = reader.GetInt32(
                        reader.GetOrdinal("ProductId")),

                    ProductName = reader.GetString(
                        reader.GetOrdinal("ProductName")),

                    Quantity = reader.GetInt32(
                        reader.GetOrdinal("Quantity")),

                    Price = reader.GetDecimal(
                        reader.GetOrdinal("Price")),

                    ItemSubtotal = reader.GetDecimal(
                        reader.GetOrdinal("ItemSubtotal")),

                    CreatedAt = reader.GetDateTime(
                        reader.GetOrdinal("CreatedAt")),

                    CreatedBy = reader.GetInt32(
                        reader.GetOrdinal("CreatedBy")),

                    UpdatedAt = reader.IsDBNull(
                        reader.GetOrdinal("UpdatedAt"))
                        ? null
                        : reader.GetDateTime(
                            reader.GetOrdinal("UpdatedAt")),

                    UpdatedBy = reader.IsDBNull(
                        reader.GetOrdinal("UpdatedBy"))
                        ? null
                        : reader.GetInt32(
                            reader.GetOrdinal("UpdatedBy"))
                });
            }

            return items;
        }

        public async Task<OrderShipping?> GetOrderShippingAsync(
            int orderId,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_GetOrderShipping",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@OrderId",
                orderId);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken);

            if (await reader.ReadAsync(
                cancellationToken))
            {
                return new OrderShipping
                {
                    OrderShippingId = reader.GetInt32(
                        reader.GetOrdinal("OrderShippingId")),

                    OrderId = reader.GetInt32(
                        reader.GetOrdinal("OrderId")),

                    ShippingName = reader.GetString(
                        reader.GetOrdinal("ShippingName")),

                    ShippingAddress = reader.GetString(
                        reader.GetOrdinal("ShippingAddress")),

                    ShippingCity = reader.GetString(
                        reader.GetOrdinal("ShippingCity")),

                    ShippingState = reader.GetString(
                        reader.GetOrdinal("ShippingState")),

                    ShippingPostalCode = reader.GetString(
                        reader.GetOrdinal("ShippingPostalCode")),

                    ShippingMethod = reader.GetString(
                        reader.GetOrdinal("ShippingMethod")),

                    ShippingCharge = reader.GetDecimal(
                        reader.GetOrdinal("ShippingCharge")),

                    CreatedAt = reader.GetDateTime(
                        reader.GetOrdinal("CreatedAt")),

                    CreatedBy = reader.GetInt32(
                        reader.GetOrdinal("CreatedBy")),

                    UpdatedAt = reader.IsDBNull(
                        reader.GetOrdinal("UpdatedAt"))
                        ? null
                        : reader.GetDateTime(
                            reader.GetOrdinal("UpdatedAt")),

                    UpdatedBy = reader.IsDBNull(
                        reader.GetOrdinal("UpdatedBy"))
                        ? null
                        : reader.GetInt32(
                            reader.GetOrdinal("UpdatedBy"))
                };
            }

            return null;
        }

        public async Task<OrderPayment?> GetOrderPaymentAsync(
            int orderId,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_GetOrderPayment",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@OrderId",
                orderId);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken);

            if (await reader.ReadAsync(
                cancellationToken))
            {
                return new OrderPayment
                {
                    PaymentId = reader.GetInt32(
                        reader.GetOrdinal("PaymentId")),

                    OrderId = reader.GetInt32(
                        reader.GetOrdinal("OrderId")),

                    PaymentStatus = reader.GetString(
                        reader.GetOrdinal("PaymentStatus")),

                    PaymentMethod = reader.IsDBNull(
                        reader.GetOrdinal("PaymentMethod"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("PaymentMethod")),

                    Amount = reader.GetDecimal(
                        reader.GetOrdinal("Amount")),

                    TransactionReference = reader.IsDBNull(
                        reader.GetOrdinal("TransactionReference"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("TransactionReference")),

                    CreatedAt = reader.GetDateTime(
                        reader.GetOrdinal("CreatedAt")),

                    CreatedBy = reader.GetInt32(
                        reader.GetOrdinal("CreatedBy")),

                    UpdatedAt = reader.IsDBNull(
                        reader.GetOrdinal("UpdatedAt"))
                        ? null
                        : reader.GetDateTime(
                            reader.GetOrdinal("UpdatedAt")),

                    UpdatedBy = reader.IsDBNull(
                        reader.GetOrdinal("UpdatedBy"))
                        ? null
                        : reader.GetInt32(
                            reader.GetOrdinal("UpdatedBy"))
                };
            }

            return null;
        }

        public async Task UpdateOrderStatusAsync(
            int orderId,
            string orderStatus,
            int updatedBy,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_UpdateOrderStatus",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@OrderId",
                orderId);

            command.Parameters.AddWithValue(
                "@OrderStatus",
                orderStatus);

            command.Parameters.AddWithValue(
                "@UpdatedBy",
                updatedBy);

            await connection.OpenAsync(
                cancellationToken);

            await command.ExecuteNonQueryAsync(
                cancellationToken);
        }

        public async Task<int> AddOrderTrackingAsync(
    OrderTracking tracking,
    CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                "Srizan_AddOrderTracking",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@OrderId", tracking.OrderId);
            command.Parameters.AddWithValue("@Status", tracking.Status);
            command.Parameters.AddWithValue(
                "@TrackingNote",
                (object?)tracking.TrackingNote ?? DBNull.Value);
            command.Parameters.AddWithValue(
                "@CreatedBy",
                (object?)tracking.CreatedBy ?? DBNull.Value);

            await connection.OpenAsync(cancellationToken);

            return Convert.ToInt32(
                await command.ExecuteScalarAsync(cancellationToken));
        }

        public async Task<List<OrderTracking>> GetOrderTrackingAsync(
    int orderId,
    CancellationToken cancellationToken)
        {
            var trackingList = new List<OrderTracking>();

            using var connection = new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                "Srizan_GetOrderTracking",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@OrderId", orderId);

            await connection.OpenAsync(cancellationToken);

            using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                trackingList.Add(new OrderTracking
                {
                    TrackingId = reader.GetInt32(
                        reader.GetOrdinal("TrackingId")),

                    OrderId = reader.GetInt32(
                        reader.GetOrdinal("OrderId")),

                    Status = reader.GetString(
                        reader.GetOrdinal("Status")),

                    TrackingNote = reader.IsDBNull(
                        reader.GetOrdinal("TrackingNote"))
                        ? null
                        : reader.GetString(
                            reader.GetOrdinal("TrackingNote")),

                    CreatedAt = reader.GetDateTime(
                        reader.GetOrdinal("CreatedAt")),

                    CreatedBy = reader.IsDBNull(
                        reader.GetOrdinal("CreatedBy"))
                        ? null
                        : reader.GetInt32(
                            reader.GetOrdinal("CreatedBy"))
                });
            }

            return trackingList;
        }

        public async Task<(List<Order> Orders, int TotalRecords)> GetOrderHistoryAsync(
    int customerId,
    string? status,
    int page,
    int pageSize,
    CancellationToken cancellationToken)
        {
            var orders = new List<Order>();
            var totalRecords = 0;

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_GetOrderHistory", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@CustomerId", customerId);

            command.Parameters.AddWithValue(
                "@Status",
                (object?)status ?? DBNull.Value);

            command.Parameters.AddWithValue("@Page", page);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                if (totalRecords == 0)
                {
                    totalRecords =
                        reader.GetInt32(
                            reader.GetOrdinal("TotalRecords"));
                }

                orders.Add(new Order
                {
                    OrderId = reader.GetInt32(
                        reader.GetOrdinal("OrderId")),

                    CustomerId = reader.GetInt32(
                        reader.GetOrdinal("CustomerId")),

                    Subtotal = reader.GetDecimal(
                        reader.GetOrdinal("Subtotal")),

                    ShippingCharge = reader.GetDecimal(
                        reader.GetOrdinal("ShippingCharge")),

                    Tax = reader.GetDecimal(
                        reader.GetOrdinal("Tax")),

                    TotalAmount = reader.GetDecimal(
                        reader.GetOrdinal("TotalAmount")),

                    OrderStatus = reader.GetString(
                        reader.GetOrdinal("OrderStatus")),

                    CreatedAt = reader.GetDateTime(
                        reader.GetOrdinal("CreatedAt"))
                });
            }

            return (orders, totalRecords);
        }
    }
}