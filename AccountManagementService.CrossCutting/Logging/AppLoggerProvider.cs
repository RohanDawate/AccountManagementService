using AccountManagementService.CrossCutting.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace AccountManagementService.CrossCutting.Logging
{
    [NoLog]
    public static class AppLoggerProvider
    {
        private static ILoggerFactory _factory = NullLoggerFactory.Instance;

        public static void Initialize(ILoggerFactory factory)
        {
            _factory = factory ?? NullLoggerFactory.Instance;
        }

        public static ILogger CreateLogger(Type type) => _factory.CreateLogger(type);
    }
}
