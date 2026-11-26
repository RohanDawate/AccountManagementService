namespace AccountManagementService.API.Shared.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {

                var correlationId = context.Items["CorrelationId"]?.ToString() ?? context.TraceIdentifier;
                logger.LogError(ex, "Unhandled exception. CorrelationId: {CorrelationId}", correlationId);


                var (statusCode, errorCode, message) = ex switch
                {
                    UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "UNAUTHORIZED", "Access denied."),
                    KeyNotFoundException => (StatusCodes.Status404NotFound, "NOT_FOUND", ex.Message),
                    ArgumentException => (StatusCodes.Status400BadRequest, "BAD_REQUEST", ex.Message),
                    _ => (StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred.")
                };

                context.Response.Clear();
                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                //var envelope = ApiResponse<object>.Fail(errorCode, message, correlationId);
                //await context.Response.WriteAsync(envelope.ToJson());
            }
        }
    }
}
