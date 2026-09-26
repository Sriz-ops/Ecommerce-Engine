using EcommerceAPI.Models;

namespace EcommerceAPI.Repositories.Interfaces
{
    public interface IErrorLogRepository
    {
        Task LogErrorAsync(
            string httpMethod,
            string requestPath,
            string errorMessage,
            string stackTrace,
            CancellationToken cancellationToken);
    }
}