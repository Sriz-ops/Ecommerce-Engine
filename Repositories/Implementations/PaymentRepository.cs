using System.Data;
using EcommerceAPI.Models.Payment;
using EcommerceAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace EcommerceAPI.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly string _connectionString;

        public PaymentRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Database connection string is not configured.");
        }

        public async Task<int> CreatePaymentAsync(
            OrderPayment payment,
            CancellationToken cancellationToken)
        {
            await using var connection =
                new SqlConnection(_connectionString);

            await using var command =
                new SqlCommand(
                    "Srizan_CreateOrderPayment",
                    connection);

            command.CommandType =
                CommandType.StoredProcedure;

            command.Parameters.AddWithValue(
                "@OrderId",
                payment.OrderId);

            command.Parameters.AddWithValue(
                "@PaymentStatus",
                payment.PaymentStatus);

            command.Parameters.AddWithValue(
                "@PaymentMethod",
                payment.PaymentMethod ?? (object)DBNull.Value);

            command.Parameters.AddWithValue(
                "@Amount",
                payment.Amount);

            command.Parameters.AddWithValue(
                "@TransactionReference",
                payment.TransactionReference ?? (object)DBNull.Value);

            command.Parameters.AddWithValue(
                "@CreatedBy",
                payment.CreatedBy);

            await connection.OpenAsync(cancellationToken);

            var result =
                await command.ExecuteScalarAsync(
                    cancellationToken);

            return Convert.ToInt32(result);
        }
    }
}