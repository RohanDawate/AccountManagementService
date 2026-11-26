using AccountManagementService.CrossCutting.Constants;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AccountManagementService.CrossCutting.Logging
{
    public class AppLogger<T> : IAppLogger<T>
    {
        private const string BusinessCategory = "Business";
        private const string AuditCategory = "Audit";
        private const string DebugCategory = "Debug";

        private readonly ILogger<T> _logger;

        public AppLogger(ILogger<T> logger) => _logger = logger;

        // --- Business ---
        public void BusinessInfo(string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, BusinessCategory)
                  .Information(message, args);

        public void BusinessWarning(string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, BusinessCategory)
                   .Warning(message, args);

        public void BusinessWarning(Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, BusinessCategory)
                  .Warning(ex, message, args);

        public void BusinessError(string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, BusinessCategory)
                  .Error(message, args);

        public void BusinessError(Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, BusinessCategory)
                  .Error(ex, message, args);

        public void BusinessFatal(Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, BusinessCategory)
                  .Fatal(ex, message, args);

        // --- Audit ---
        public void AuditInfo(string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, AuditCategory)
                  .Information(message, args);

        public void AuditWarning(string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, AuditCategory)
                    .Warning(message, args);

        public void AuditWarning(Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, AuditCategory)
                  .Warning(ex, message, args);

        public void AuditError(string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, AuditCategory)
                  .Error(message, args);

        public void AuditError(Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, AuditCategory)
                  .Error(ex, message, args);

        public void AuditFatal(Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, AuditCategory)
                  .Fatal(ex, message, args);

        // --- Dev / Debug ---
        public void DevDebug(string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Debug(message, args);

        public void DevInfo(string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Information(message, args);

        public void DevWarning(string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Warning(message, args);

        public void DevWarning(Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Warning(ex, message, args);

        public void DevError(string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Error(message, args);

        public void DevError(Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Error(ex, message, args);

        public void DevFatal(Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Fatal(ex, message, args);

    }
}
