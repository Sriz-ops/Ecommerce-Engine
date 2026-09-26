using System.Data;
using EcommerceAPI.DTOs.Inventory;
using EcommerceAPI.Models;
using EcommerceAPI.Models.Inventory;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EcommerceAPI.Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured.");
        }

        public async Task<List<ProductDetails>> GetAllAsync(
    CancellationToken cancellationToken)
        {
            var products = new List<ProductDetails>();

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_GetAllProducts", connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                products.Add(MapProduct(reader));
            }

            return products;
        }

        public async Task<ProductDetails?> GetByIdAsync(
    int id,
    CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_GetProductById", connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@ProductId", id);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            if (await reader.ReadAsync(cancellationToken))
            {
                return MapProduct(reader);
            }

            return null;
        }

        public async Task<ProductPagedResult> SearchAsync(
     string? search,
     int? categoryId,
     int? brandId,
     decimal? minPrice,
     decimal? maxPrice,
     decimal? minRating,
     string? sortBy,
     string? sortOrder,
     int page,
     int pageSize,
     CancellationToken cancellationToken)
        {
            var products = new List<ProductDetails>();

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_SearchProducts", connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@Search",
                (object?)search ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@CategoryId",
                (object?)categoryId ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@BrandId",
                (object?)brandId ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@MinPrice",
                (object?)minPrice ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@MaxPrice",
                (object?)maxPrice ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@MinRating",
                (object?)minRating ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@SortBy",
                (object?)sortBy ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@SortOrder",
                (object?)sortOrder ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@Page",
                page);

            command.Parameters.AddWithValue(
                "@PageSize",
                pageSize);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            var totalRecords = 0;

            while (await reader.ReadAsync(cancellationToken))
            {
                if (totalRecords == 0)
                {
                    totalRecords = reader.GetInt32(
                        reader.GetOrdinal("TotalRecords"));
                }

                products.Add(MapProduct(reader));
            }

            var totalPages =
                pageSize > 0
                    ? (int)Math.Ceiling(
                        (double)totalRecords / pageSize)
                    : 0;

            return new ProductPagedResult
            {
                Products = products,
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };
        }

        public async Task<int> CreateAsync(Product product, CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_CreateProduct", connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@Name", product.Name);

            command.Parameters.AddWithValue(
                "@Description",
                (object?)product.Description ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@CategoryId",
                product.CategoryId);

            command.Parameters.AddWithValue(
                "@BrandId",
                product.BrandId);

            command.Parameters.AddWithValue(
                "@Rating",
                product.Rating);

            await connection.OpenAsync(cancellationToken);

            var result = await command.ExecuteScalarAsync(cancellationToken);

            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(int id, Product product, CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_UpdateProduct", connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@ProductId", id);
            command.Parameters.AddWithValue("@Name", product.Name);

            command.Parameters.AddWithValue(
                "@Description",
                (object?)product.Description ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@CategoryId",
                product.CategoryId);

            command.Parameters.AddWithValue(
                "@BrandId",
                product.BrandId);

            command.Parameters.AddWithValue(
                "@Rating",
                product.Rating);

            await connection.OpenAsync(cancellationToken);

            var result = await command.ExecuteScalarAsync(cancellationToken);

            var rowsAffected = Convert.ToInt32(result);

            return rowsAffected > 0;
        }


        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_DeleteProduct", connection);

            command.CommandType = System.Data.CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@ProductId", id);

            await connection.OpenAsync(cancellationToken);

            var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);

            return rowsAffected > 0;
        }

        private static ProductDetails MapProduct(SqlDataReader reader)
        {
            return new ProductDetails
            {
                ProductId = reader.GetInt32(
                    reader.GetOrdinal("ProductId")),

                Name = reader.GetString(
                    reader.GetOrdinal("Name")),

                Description = reader.IsDBNull(
                    reader.GetOrdinal("Description"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("Description")),

                CategoryId = reader.GetInt32(
                    reader.GetOrdinal("CategoryId")),

                CategoryName = reader.GetString(
                    reader.GetOrdinal("CategoryName")),

                BrandId = reader.GetInt32(
                    reader.GetOrdinal("BrandId")),

                BrandName = reader.GetString(
                    reader.GetOrdinal("BrandName")),

                Price = reader.GetDecimal(
                    reader.GetOrdinal("Price")),

                Rating = reader.GetDecimal(
                    reader.GetOrdinal("Rating")),

                ImagePath = reader.IsDBNull(
                    reader.GetOrdinal("ImagePath"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("ImagePath"))
            };
        }

        public async Task<Inventory?> UpdateStockAsync(
    int productId,
    int quantity,
    CancellationToken cancellationToken)
        {
            using var connection = new SqlConnection(_connectionString);

            using var command = new SqlCommand(
                "Srizan_UpdateProductStock",
                connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@ProductId",
                productId);

            command.Parameters.AddWithValue(
                "@Quantity",
                quantity);

            await connection.OpenAsync(
                cancellationToken);

            using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return new Inventory
            {
                InventoryId =
                    reader.GetInt32(
                        reader.GetOrdinal("InventoryId")),

                ProductId =
                    reader.GetInt32(
                        reader.GetOrdinal("ProductId")),

                Quantity =
                    reader.GetInt32(
                        reader.GetOrdinal("Quantity")),

                UpdatedAt =
                    reader.GetDateTime(
                        reader.GetOrdinal("UpdatedAt"))
            };
        }

        public async Task<List<Inventory>> BulkUpdateStockAsync(
    List<BulkInventoryUpdateDto> items,
    CancellationToken cancellationToken)
        {
            using var connection =
                new SqlConnection(_connectionString);

            using var command =
                new SqlCommand(
                    "Srizan_BulkUpdateInventory",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            var table =
                new DataTable();

            table.Columns.Add(
                "ProductId",
                typeof(int));

            table.Columns.Add(
                "Quantity",
                typeof(int));

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
                "Srizan_BulkInventoryType";

            await connection.OpenAsync(
                cancellationToken);

            using var reader =
                await command.ExecuteReaderAsync(
                    cancellationToken);

            var inventoryList =
                new List<Inventory>();

            while (await reader.ReadAsync(
                cancellationToken))
            {
                inventoryList.Add(
                    new Inventory
                    {
                        InventoryId =
                            reader.GetInt32(
                                reader.GetOrdinal(
                                    "InventoryId")),

                        ProductId =
                            reader.GetInt32(
                                reader.GetOrdinal(
                                    "ProductId")),

                        Quantity =
                            reader.GetInt32(
                                reader.GetOrdinal(
                                    "Quantity")),

                        UpdatedAt =
                            reader.GetDateTime(
                                reader.GetOrdinal(
                                    "UpdatedAt"))
                    });
            }

            return inventoryList;
        }

        public async Task<bool> UpdateImagePathAsync(
    int id,
    string imagePath,
    CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(
                "Srizan_UpdateProductImage",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@ProductId", id);
            command.Parameters.AddWithValue("@ImagePath", imagePath);

            await connection.OpenAsync(cancellationToken);

            var rows = await command.ExecuteNonQueryAsync(cancellationToken);

            return rows > 0;
        }
    }
}