using EcommerceAPI.Models;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EcommerceAPI.Repositories.Implementations
{
    public class CartRepository : ICartRepository
    {
        private readonly string _connectionString;

        public CartRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured.");
        }

        public async Task<List<CartItem>> GetCartAsync(int customerId, CancellationToken cancellationToken)
        {
            var items = new List<CartItem>();

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_GetCart", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@CustomerId", customerId);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                items.Add(new CartItem
                {
                    CartItemId = reader.GetInt32(
                        reader.GetOrdinal("CartItemId")),

                    ProductId = reader.GetInt32(
                        reader.GetOrdinal("ProductId")),

                    ProductName = reader.GetString(
                        reader.GetOrdinal("ProductName")),

                    Quantity = reader.GetInt32(
                        reader.GetOrdinal("Quantity")),

                    Price = reader.GetDecimal(
                        reader.GetOrdinal("Price")),

                    ItemSubtotal = reader.GetDecimal(
                        reader.GetOrdinal("ItemSubtotal"))
                });
            }

            return items;
        }

        public async Task AddItemAsync(
            int customerId,
            int productId,
            int quantity,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_AddCartItem", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@CustomerId", customerId);

            command.Parameters.AddWithValue(
                "@ProductId", productId);

            command.Parameters.AddWithValue(
                "@Quantity", quantity);

            await connection.OpenAsync(cancellationToken);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task<bool> UpdateItemAsync(
            int customerId,
            int productId,
            int quantity,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_UpdateCartItem", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@CustomerId", customerId);

            command.Parameters.AddWithValue(
                "@ProductId", productId);

            command.Parameters.AddWithValue(
                "@Quantity", quantity);

            await connection.OpenAsync(cancellationToken);

            var result =
                await command.ExecuteScalarAsync(cancellationToken);

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> RemoveItemAsync(
            int customerId,
            int productId,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_RemoveCartItem", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@CustomerId", customerId);

            command.Parameters.AddWithValue(
                "@ProductId", productId);

            await connection.OpenAsync(cancellationToken);

            var result =
                await command.ExecuteScalarAsync(cancellationToken);

            return Convert.ToInt32(result) > 0;
        }

        public async Task ClearCartAsync(int customerId, CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_ClearCart", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@CustomerId", customerId);

            await connection.OpenAsync(cancellationToken);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public async Task<int> GetItemCountAsync(int customerId, CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_GetCartItemCount",
                    connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@CustomerId", customerId);

            await connection.OpenAsync(cancellationToken);

            var result =
                await command.ExecuteScalarAsync(cancellationToken);

            return Convert.ToInt32(result);
        }

        public async Task<decimal> GetSubtotalAsync(int customerId, CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_GetCartSubtotal",
                    connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@CustomerId", customerId);

            await connection.OpenAsync(cancellationToken);

            var result =
                await command.ExecuteScalarAsync(cancellationToken);

            return Convert.ToDecimal(result);
        }
    }
}