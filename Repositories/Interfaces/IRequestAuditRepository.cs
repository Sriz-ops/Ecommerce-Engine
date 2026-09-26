namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IRequestAuditRepository
    {
        Task LogRequestAsync(
            string? userId,
            string? username,
            string httpMethod,
            string requestPath,
            string? requestParameters,
            int responseStatus,
            CancellationToken cancellationToken);
    }
}