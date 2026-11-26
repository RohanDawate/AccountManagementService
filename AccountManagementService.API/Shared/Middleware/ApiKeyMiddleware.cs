using AccountManagementService.API.Shared.Helpers;
using AccountManagementService.CrossCutting.Attributes;

namespace AccountManagementService.API.Shared.Middleware
{
    [NoLog]
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private const string ApiKeyHeaderName = "X-Api-Key";

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Optional: skip for swagger / health endpoints
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/scalar") ||
                context.Request.Path.StartsWithSegments("/health"))
            {
                await _next(context);
                return;
            }

            var (isRequired, allowedClients) = EndPointMetadataHelper.GetApiKeySettings(context);
            if (!isRequired)
            {
                await _next(context);
                return;
            }

            //Read HeaderName
            var headerName = _configuration.GetValue<string>("ApiKeySettings:HeaderName") ?? "X-Api-Key";
            if (!context.Request.Headers.TryGetValue(headerName, out var extractedApiKey) ||
                string.IsNullOrWhiteSpace(extractedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Api Key is missing");
                return;
            }


            // Read clients directly
            var clients = _configuration
                .GetSection("ApiKeySettings:Clients")
                .Get<Dictionary<string, string>>();

            if (clients is null || clients.Count == 0)
            {
                // Helps you immediately see if configuration failed to load
                throw new InvalidOperationException("ApiKeySettings:Clients is missing or empty in configuration.");
            }

            var incomingKey = extractedApiKey.ToString().Trim();
            var matchedClient = clients.FirstOrDefault(c =>
                string.Equals(c.Value, incomingKey, StringComparison.Ordinal));

            if (string.IsNullOrEmpty(matchedClient.Key))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid Api Key");
                return;
            }

            context.Items["ApiClientName"] = matchedClient.Key;

            await _next(context);
        }
    }
}
