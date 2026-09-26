using EcommerceAPI.Models;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EcommerceAPI.Repositories.Implementations
{
    public class ProductPriceRepository : IProductPriceRepository
    {
        private readonly string _connectionString;

        public ProductPriceRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured.");
        }

        public async Task<ProductPrice?> GetByIdAsync(int id)
        {
            const string query = """
                SELECT
                    ProductPriceId,
                    ProductId,
                    Price,
                    CreatedAt
                FROM Srizan_ProductPrices
                WHERE ProductPriceId = @ProductPriceId;
                """;

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ProductPriceId", id);

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapProductPrice(reader);
            }

            return null;
        }

        public async Task<ProductPrice?> GetCurrentPriceByProductIdAsync(
            int productId)
        {
            const string query = """
                SELECT TOP 1
                    ProductPriceId,
                    ProductId,
                    Price,
                    CreatedAt
                FROM Srizan_ProductPrices
                WHERE ProductId = @ProductId
                ORDER BY CreatedAt DESC;
                """;

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ProductId", productId);

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapProductPrice(reader);
            }

            return null;
        }

        public async Task<int> AddAsync(ProductPrice productPrice)
        {
            const string query = """
                INSERT INTO Srizan_ProductPrices
                    (ProductId, Price)
                OUTPUT INSERTED.ProductPriceId
                VALUES
                    (@ProductId, @Price);
                """;

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(query, connection);

            command.Parameters.AddWithValue(
                "@ProductId",
                productPrice.ProductId);

            command.Parameters.AddWithValue(
                "@Price",
                productPrice.Price);

            await connection.OpenAsync();

            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }

        private static ProductPrice MapProductPrice(
            SqlDataReader reader)
        {
            return new ProductPrice
            {
                ProductPriceId =
                    reader.GetInt32(
                        reader.GetOrdinal("ProductPriceId")),

                ProductId =
                    reader.GetInt32(
                        reader.GetOrdinal("ProductId")),

                Price =
                    reader.GetDecimal(
                        reader.GetOrdinal("Price")),

                CreatedAt =
                    reader.GetDateTime(
                        reader.GetOrdinal("CreatedAt"))
            };
        }
    }
}