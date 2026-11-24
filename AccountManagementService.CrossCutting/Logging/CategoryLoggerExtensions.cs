using AccountManagementService.CrossCutting.Attributes;
using AccountManagementService.CrossCutting.Constants;
using Serilog;

namespace AccountManagementService.CrossCutting.Logging
{
    [NoLog]
    public static class CategoryLoggerExtensions
    {
        private const string BusinessCategory = "Business";
        private const string AuditCategory = "Audit";
        private const string DebugCategory = "Debug";

        // --- Business ---
        public static void BusinessInfo(this Microsoft.Extensions.Logging.ILogger logger, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, BusinessCategory)
                  .Information(message, args);

        public static void BusinessWarning(this Microsoft.Extensions.Logging.ILogger logger, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, BusinessCategory)
                   .Warning(message, args);

        public static void BusinessWarning(this Microsoft.Extensions.Logging.ILogger logger, Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, BusinessCategory)
                  .Warning(ex, message, args);

        public static void BusinessError(this Microsoft.Extensions.Logging.ILogger logger, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, BusinessCategory)
                  .Error(message, args);

        public static void BusinessError(this Microsoft.Extensions.Logging.ILogger logger, Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, BusinessCategory)
                  .Error(ex, message, args);

        public static void BusinessFatal(this Microsoft.Extensions.Logging.ILogger logger, Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, BusinessCategory)
                  .Fatal(ex, message, args);

        // --- Audit ---
        public static void AuditInfo(this Microsoft.Extensions.Logging.ILogger logger, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, AuditCategory)
                  .Information(message, args);

        public static void AuditWarning(this Microsoft.Extensions.Logging.ILogger logger, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, AuditCategory)
                    .Warning(message, args);

        public static void AuditWarning(this Microsoft.Extensions.Logging.ILogger logger, Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, AuditCategory)
                  .Warning(ex, message, args);

        public static void AuditError(this Microsoft.Extensions.Logging.ILogger logger, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, AuditCategory)
                  .Error(message, args);

        public static void AuditError(this Microsoft.Extensions.Logging.ILogger logger, Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, AuditCategory)
                  .Error(ex, message, args);

        public static void AuditFatal(this Microsoft.Extensions.Logging.ILogger logger, Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, AuditCategory)
                  .Fatal(ex, message, args);

        // --- Dev / Debug ---
        public static void DevDebug(this Microsoft.Extensions.Logging.ILogger logger, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Debug(message, args);

        public static void DevInfo(this Microsoft.Extensions.Logging.ILogger logger, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Information(message, args);

        public static void DevWarning(this Microsoft.Extensions.Logging.ILogger logger, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Warning(message, args);

        public static void DevWarning(this Microsoft.Extensions.Logging.ILogger logger, Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Warning(ex, message, args);

        public static void DevError(this Microsoft.Extensions.Logging.ILogger logger, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Error(message, args);

        public static void DevError(this Microsoft.Extensions.Logging.ILogger logger, Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Error(ex, message, args);

        public static void DevFatal(this Microsoft.Extensions.Logging.ILogger logger, Exception? ex, string message, params object?[] args)
            => Log.ForContext(LogCategoryConstants.PropertyName, DebugCategory)
                  .Fatal(ex, message, args);
    }
}
