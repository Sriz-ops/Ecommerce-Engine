using EcommerceAPI.Models;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EcommerceAPI.Repositories.Implementations
{
    public class BrandRepository : IBrandRepository
    {
        private readonly string _connectionString;

        public BrandRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured.");
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            var brands = new List<Brand>();

            const string query = """
                SELECT BrandId, Name
                FROM Srizan_Brands
                ORDER BY Name;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                brands.Add(new Brand
                {
                    BrandId = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return brands;
        }

        public async Task<Brand?> GetByIdAsync(int id)
        {
            const string query = """
                SELECT BrandId, Name
                FROM Srizan_Brands
                WHERE BrandId = @BrandId;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@BrandId", id);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Brand
                {
                    BrandId = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
            }

            return null;
        }
    }
}