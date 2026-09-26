using EcommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace EcommerceAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (SqlException ex) when (
    ex.Number == 50001 ||
    ex.Number == 50002 ||
    ex.Number == 50003 ||
    ex.Number == 50004 ||
    ex.Number == 50005 ||
    ex.Number == 50006 ||
    ex.Number == 50012 ||
    ex.Number == 50013)
            {
                _logger.LogWarning(
                    ex,
                    "Business validation failed for {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                var errorRepository =
                    context.RequestServices
                        .GetRequiredService<IErrorLogRepository>();

                await errorRepository.LogErrorAsync(
                    context.Request.Method,
                    context.Request.Path,
                    ex.Message,
                    ex.StackTrace ?? string.Empty,
                    context.RequestAborted);

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/problem+json";

                var problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation error",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };

                await context.Response.WriteAsJsonAsync(problemDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred while processing {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                var errorRepository =
                    context.RequestServices
                        .GetRequiredService<IErrorLogRepository>();

                await errorRepository.LogErrorAsync(
                    context.Request.Method,
                    context.Request.Path,
                    ex.Message,
                    ex.StackTrace ?? string.Empty,
                    context.RequestAborted);

                await HandleExceptionAsync(context);
            }
        }

        private static async Task HandleExceptionAsync(
            HttpContext context)
        {
            context.Response.Clear();

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            context.Response.ContentType =
                "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = "The server encountered an unexpected error.",
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(
                problemDetails);
        }
    }
}