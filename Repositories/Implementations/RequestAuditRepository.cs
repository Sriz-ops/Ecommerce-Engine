using EcommerceAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace EcommerceAPI.Repositories.Implementations
{
    public class RequestAuditRepository : IRequestAuditRepository
    {
        private readonly string _connectionString;

        public RequestAuditRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured.");
        }

        public async Task LogRequestAsync(
            string? userId,
            string? username,
            string httpMethod,
            string requestPath,
            string? requestParameters,
            int responseStatus,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand("Srizan_LogRequest", connection);

            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@UserId",
                (object?)userId ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@Username",
                (object?)username ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@HttpMethod",
                httpMethod);

            command.Parameters.AddWithValue(
                "@RequestPath",
                requestPath);

            command.Parameters.AddWithValue(
                "@RequestParameters",
                (object?)requestParameters ?? DBNull.Value);

            command.Parameters.AddWithValue(
                "@ResponseStatus",
                responseStatus);

            await connection.OpenAsync(cancellationToken);

            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}