using AccountManagementService.CrossCutting.Attributes;
using AccountManagementService.CrossCutting.Constants;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace AccountManagementService.API.Middleware
{
    [NoLog]
    public partial class CorrelationAndTraceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationAndTraceMiddleware> _logger;

        [GeneratedRegex(@"^[a-zA-Z0-9\-_]{1,64}$", RegexOptions.Compiled)]
        private static partial Regex SafeIdRegex();

        public CorrelationAndTraceMiddleware(
            RequestDelegate next,
            ILogger<CorrelationAndTraceMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            var incomingCorrelationId = context.Request.Headers[HeaderConstants.CorrelationId].FirstOrDefault();
            var correlationId = !string.IsNullOrWhiteSpace(incomingCorrelationId) && SafeIdRegex().IsMatch(incomingCorrelationId)
                ? incomingCorrelationId
                : Guid.NewGuid().ToString("N");

            // ASP.NET Core automatically creates Activity.Current for HTTP requests; fallback to GUID if null
            var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
            context.TraceIdentifier = traceId;

            context.Items["CorrelationId"] = correlationId;
            context.Items["TraceId"] = traceId;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderConstants.CorrelationId] = correlationId;
                context.Response.Headers[HeaderConstants.TraceId] = traceId;
                return Task.CompletedTask;
            });

            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId,
                ["TraceId"] = traceId
            }))
            {
                await _next(context);
            }
        }
    }
}
