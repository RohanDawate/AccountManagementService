using AccountManagementService.CrossCutting.Attributes;
using AccountManagementService.CrossCutting.Interfaces;
using System.Diagnostics;

namespace AccountManagementService.API.Services
{
    [NoLog]
    public class TraceContext : ITraceContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TraceContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string CorrelationId =>
            _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString()
            ?? Guid.NewGuid().ToString("N");

        public string TraceId =>
            _httpContextAccessor.HttpContext?.Items["TraceId"]?.ToString()
            ?? Activity.Current?.TraceId.ToString()
            ?? Guid.NewGuid().ToString("N");
    }
}
