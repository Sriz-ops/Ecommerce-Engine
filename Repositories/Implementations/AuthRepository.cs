using EcommerceAPI.Models;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EcommerceAPI.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly string _connectionString;

        public AuthRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured.");
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_GetCustomerByEmail",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@Email",
                email);

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new Customer
            {
                CustomerId = reader.GetInt32(
                    reader.GetOrdinal("CustomerId")),

                Name = reader.GetString(
                    reader.GetOrdinal("Name")),

                Email = reader.GetString(
                    reader.GetOrdinal("Email")),

                PasswordHash = reader.GetString(
                    reader.GetOrdinal("PasswordHash")),

                Role = reader.GetString(
                    reader.GetOrdinal("Role")),

                CreatedAt = reader.GetDateTime(
                    reader.GetOrdinal("CreatedAt"))
            };
        }

        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_GetCustomerById",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@CustomerId",
                customerId);

            await connection.OpenAsync();

            await using var reader =
                await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new Customer
            {
                CustomerId = reader.GetInt32(
                    reader.GetOrdinal("CustomerId")),

                Name = reader.GetString(
                    reader.GetOrdinal("Name")),

                Email = reader.GetString(
                    reader.GetOrdinal("Email")),

                PasswordHash = reader.GetString(
                    reader.GetOrdinal("PasswordHash")),

                Role = reader.GetString(
                    reader.GetOrdinal("Role")),

                CreatedAt = reader.GetDateTime(
                    reader.GetOrdinal("CreatedAt"))
            };
        }

        public async Task<int> CreateCustomerAsync(
            Customer customer)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_CreateCustomer",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@Name",
                customer.Name);

            command.Parameters.AddWithValue(
                "@Email",
                customer.Email);

            command.Parameters.AddWithValue(
                "@PasswordHash",
                customer.PasswordHash);

            command.Parameters.AddWithValue(
                "@Role",
                customer.Role);

            await connection.OpenAsync();

            var result =
                await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }
    }
}