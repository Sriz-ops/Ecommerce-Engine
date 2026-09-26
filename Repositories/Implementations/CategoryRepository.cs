using EcommerceAPI.Models;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EcommerceAPI.Repositories.Implementations
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured.");
        }

        public async Task<List<Category>> GetAllAsync()
        {
            var categories = new List<Category>();

            const string query = """
                SELECT CategoryId, Name
                FROM Srizan_Categories
                ORDER BY Name;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categories.Add(new Category
                {
                    CategoryId = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return categories;
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            const string query = """
                SELECT CategoryId, Name
                FROM Srizan_Categories
                WHERE CategoryId = @CategoryId;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await using var command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@CategoryId", id);

            await connection.OpenAsync();

            await using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new Category
                {
                    CategoryId = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };
            }

            return null;
        }
    }
}