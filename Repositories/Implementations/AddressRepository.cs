using System.Data;
using EcommerceAPI.Models.Address;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EcommerceAPI.Repositories.Implementations
{
    public class AddressRepository : IAddressRepository
    {
        private readonly string _connectionString;

        public AddressRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured.");
        }

        public async Task<int> CreateAsync(
            Address address,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_CreateAddress", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@CustomerId", address.CustomerId);

            command.Parameters.AddWithValue(
                "@AddressLine1", address.AddressLine1);

            command.Parameters.AddWithValue(
                "@AddressLine2",
                (object?)address.AddressLine2 ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@City", address.City);

            command.Parameters.AddWithValue(
                "@State", address.State);

            command.Parameters.AddWithValue(
                "@PostalCode", address.PostalCode);

            command.Parameters.AddWithValue(
                "@Country", address.Country);

            command.Parameters.AddWithValue(
                "@IsDefault", address.IsDefault);

            await connection.OpenAsync(cancellationToken);

            var result =
                await command.ExecuteScalarAsync(cancellationToken);

            return Convert.ToInt32(result);
        }

        public async Task<List<Address>> GetByCustomerIdAsync(
            int customerId,
            CancellationToken cancellationToken)
        {
            var addresses = new List<Address>();

            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_GetCustomerAddresses",
                    connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@CustomerId", customerId);

            await connection.OpenAsync(cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                addresses.Add(MapAddress(reader));
            }

            return addresses;
        }

        public async Task<Address?> GetByIdAsync(
            int addressId,
            int customerId,
            CancellationToken cancellationToken)
        {
            var addresses =
                await GetByCustomerIdAsync(
                    customerId,
                    cancellationToken);

            return addresses.FirstOrDefault(
                x => x.AddressId == addressId);
        }

        public async Task<bool> UpdateAsync(
            Address address,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_UpdateAddress",
                    connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@AddressId", address.AddressId);

            command.Parameters.AddWithValue(
                "@CustomerId", address.CustomerId);

            command.Parameters.AddWithValue(
                "@AddressLine1", address.AddressLine1);

            command.Parameters.AddWithValue(
                "@AddressLine2",
                (object?)address.AddressLine2 ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@City", address.City);

            command.Parameters.AddWithValue(
                "@State", address.State);

            command.Parameters.AddWithValue(
                "@PostalCode", address.PostalCode);

            command.Parameters.AddWithValue(
                "@Country", address.Country);

            command.Parameters.AddWithValue(
                "@IsDefault", address.IsDefault);

            await connection.OpenAsync(cancellationToken);

            var result =
                await command.ExecuteScalarAsync(cancellationToken);

            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> DeleteAsync(
            int addressId,
            int customerId,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_DeleteAddress",
                    connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@AddressId", addressId);

            command.Parameters.AddWithValue(
                "@CustomerId", customerId);

            await connection.OpenAsync(cancellationToken);

            var result =
                await command.ExecuteScalarAsync(cancellationToken);

            return Convert.ToInt32(result) > 0;
        }

        private static Address MapAddress(SqlDataReader reader)
        {
            return new Address
            {
                AddressId = reader.GetInt32(
                    reader.GetOrdinal("AddressId")),

                CustomerId = reader.GetInt32(
                    reader.GetOrdinal("CustomerId")),

                AddressLine1 = reader.GetString(
                    reader.GetOrdinal("AddressLine1")),

                AddressLine2 = reader.IsDBNull(
                    reader.GetOrdinal("AddressLine2"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("AddressLine2")),

                City = reader.GetString(
                    reader.GetOrdinal("City")),

                State = reader.GetString(
                    reader.GetOrdinal("State")),

                PostalCode = reader.GetString(
                    reader.GetOrdinal("PostalCode")),

                Country = reader.GetString(
                    reader.GetOrdinal("Country")),

                IsDefault = reader.GetBoolean(
                    reader.GetOrdinal("IsDefault")),

                CreatedAt = reader.GetDateTime(
                    reader.GetOrdinal("CreatedAt")),

                UpdatedAt = reader.IsDBNull(
                    reader.GetOrdinal("UpdatedAt"))
                    ? null
                    : reader.GetDateTime(
                        reader.GetOrdinal("UpdatedAt"))
            };
        }
    }
}