
using Microsoft.AspNetCore.Mvc;

namespace FieldStaff.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
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
            catch (Exception ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogError(
                        ex,
                        "Exception occurred after response started. TraceId: {TraceId}",
                        context.TraceIdentifier);

                    throw;
                }

                _logger.LogError(
                    ex,
                    "Unhandled exception occurred. Method: {Method}, Path: {Path}, TraceId: {TraceId}",
                    context.Request.Method,
                    context.Request.Path,
                    context.TraceIdentifier);

                context.Response.Clear();

                var (statusCode, title, detail) = ex switch
                {
                    BadHttpRequestException =>
                        (400, "Bad Request",
                            "The request is invalid."),

                    UnauthorizedAccessException =>
                        (403, "Forbidden",
                            "You are not allowed to access this resource."),

                    KeyNotFoundException =>
                        (404, "Not Found",
                            "The requested resource was not found."),

                    ArgumentException =>
                        (400, "Bad Request",
                            "The supplied arguments are invalid."),

                    InvalidOperationException =>
                        (409, "Conflict",
                            "The requested operation cannot be completed."),

                    _ =>
                        (500, "Internal Server Error",
                            "An unexpected error occurred. Please try again.")
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Detail = detail,
                    Instance = context.Request.Path
                };

                problem.Extensions["traceId"] =
                    context.TraceIdentifier;

                await context.Response.WriteAsJsonAsync(
                    problem,
                    cancellationToken: context.RequestAborted);
            }
        }
    }
}
