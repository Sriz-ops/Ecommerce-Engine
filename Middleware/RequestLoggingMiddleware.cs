using System.Diagnostics;
using System.Security.Claims;
using EcommerceAPI.Repositories.Interfaces;

namespace EcommerceAPI.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                var userId =
                    context.User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                var username =
                    context.User.Identity?.Name;

                var requestParameters =
                    context.Request.QueryString.HasValue
                        ? context.Request.QueryString.Value
                        : null;

                _logger.LogInformation(
                    "HTTP {Method} {Endpoint} responded with {StatusCode} in {Duration} ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                var auditRepository =
                    context.RequestServices
                        .GetRequiredService<IRequestAuditRepository>();

                await auditRepository.LogRequestAsync(
                    userId,
                    username,
                    context.Request.Method,
                    context.Request.Path,
                    requestParameters,
                    context.Response.StatusCode,
                    context.RequestAborted);
            }
        }
    }
}