using EcommerceAPI.Models;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EcommerceAPI.Repositories.Implementations
{
    public class CheckoutRepository : ICheckoutRepository
    {
        private readonly string _connectionString;

        public CheckoutRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured.");
        }

        public async Task<List<CartItem>> GetCartItemsAsync(
            int customerId,
            CancellationToken cancellationToken)
        {
            var items = new List<CartItem>();

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_GetCart",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@CustomerId",
                customerId);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken);

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
    }
}