using EcommerceAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EcommerceAPI.Repositories.Implementations
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly string _connectionString;

        public ErrorLogRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured.");
        }

        public async Task LogErrorAsync(
            string httpMethod,
            string requestPath,
            string errorMessage,
            string stackTrace,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_LogError", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue("@HttpMethod", httpMethod);
            command.Parameters.AddWithValue("@RequestPath", requestPath);
            command.Parameters.AddWithValue("@ErrorMessage", errorMessage);
            command.Parameters.AddWithValue("@StackTrace", stackTrace);

            await connection.OpenAsync(cancellationToken);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}