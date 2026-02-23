using System.Net;
using System.Text.Json;
using GamesService.Exceptions;

namespace GamesService.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorResponse = new ErrorResponse
            {
                Success = false
            };

            switch (exception)
            {
                case NotFoundException notFoundEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.Message = notFoundEx.Message;
                    _logger.LogWarning(notFoundEx, "Resource not found");
                    break;

                case BadRequestException badRequestEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = badRequestEx.Message;
                    _logger.LogWarning(badRequestEx, "Bad request");
                    break;

                case ArgumentNullException argNullEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.Message = argNullEx.Message;
                    _logger.LogWarning(argNullEx, "Argument null exception");
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.Message = "An unexpected error occurred. Please try again later.";
                    _logger.LogError(exception, "Unhandled exception occurred");
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(result);
        }
    }

    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
