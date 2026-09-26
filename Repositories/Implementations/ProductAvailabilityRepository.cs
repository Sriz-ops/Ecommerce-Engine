using System.Data;
using System.Threading;
using EcommerceAPI.Models;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EcommerceAPI.Repositories.Implementations
{
    public class ProductAvailabilityRepository
        : IProductAvailabilityRepository
    {
        private readonly string _connectionString;

        public ProductAvailabilityRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured.");
        }

        public async Task<ProductAvailability?> GetAvailabilityAsync(
            int productId, CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_GetProductAvailability",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@ProductId",
                productId);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            if (await reader.ReadAsync(cancellationToken))
            {
                return new ProductAvailability
                {
                    ProductId = reader.GetInt32(
                        reader.GetOrdinal("ProductId")),

                    StockQuantity = reader.GetInt32(
                        reader.GetOrdinal("StockQuantity")),

                    IsAvailable = reader.GetBoolean(
                        reader.GetOrdinal("IsAvailable"))
                };
            }

            return null;
        }

        public async Task<List<InventoryCheckResult>>
            CheckInventoryAsync(List<InventoryCheckItem> items, CancellationToken cancellationToken)
        {
            var results = new List<InventoryCheckResult>();

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_CheckInventory",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            var table = new DataTable();

            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("Quantity", typeof(int));

            foreach (var item in items)
            {
                table.Rows.Add(
                    item.ProductId,
                    item.Quantity);
            }

            var parameter =
                command.Parameters.AddWithValue(
                    "@InventoryItems",
                    table);

            parameter.SqlDbType =
                SqlDbType.Structured;

            parameter.TypeName =
                "Srizan_InventoryItemType";

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(new InventoryCheckResult
                {
                    ProductId = reader.GetInt32(
                        reader.GetOrdinal("ProductId")),

                    RequestedQuantity = reader.GetInt32(
                        reader.GetOrdinal("RequestedQuantity")),

                    AvailableQuantity = reader.GetInt32(
                        reader.GetOrdinal("AvailableQuantity")),

                    IsAvailable = reader.GetBoolean(
                        reader.GetOrdinal("IsAvailable"))
                });
            }

            return results;
        }
    }
}