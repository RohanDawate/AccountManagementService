using AccountManagementService.CrossCutting.Attributes;
using AccountManagementService.CrossCutting.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Templates;

namespace AccountManagementService.CrossCutting.Logging
{
    [NoLog]
    public static class SerilogBootstrap
    {
        private const string AuditJsonTemplate =
            "{ {timestamp: UtcDateTime(@t), category: 'AUDIT', action: AuditAction, entity: EntityName, entityId: EntityId, status: AuditStatus, userId: UserId, details: AuditDetails, machine: MachineName} }\n";

        private const string BusinessJsonTemplate =
            "{ {timestamp: UtcDateTime(@t), level: @l, category: 'BUSINESS', source: SourceContext, method: Method, elapsedMs: ElapsedMs, message: @m, args: Args, exception: @x} }\n";

        private const string DebugJsonTemplate =
            "{ {timestamp: UtcDateTime(@t), level: @l, category: 'DEBUG', threadId: ThreadId, source: SourceContext, message: @m, properties: @p, exception: @x} }\n";

        public static WebApplicationBuilder AddEnterpriseLogging(this WebApplicationBuilder builder)
        {
            var auditFormatter = new ExpressionTemplate(AuditJsonTemplate);
            var businessFormatter = new ExpressionTemplate(BusinessJsonTemplate);
            var debugFormatter = new ExpressionTemplate(DebugJsonTemplate);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProperty(LogCategoryConstants.PropertyName, LogCategoryConstants.Business)

                // 1. Console Sink (Information+)
                .WriteTo.Console(
                    restrictedToMinimumLevel: LogEventLevel.Information,
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{LogCategory}] {Message:lj}{NewLine}{Exception}")

                // 2. Audit Log File (JSON, 365 days retention)
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(Matching.WithProperty(LogCategoryConstants.PropertyName, LogCategoryConstants.Audit))
                    .WriteTo.Async(a => a.File(
                        formatter: auditFormatter,
                        path: "logs/audit/audit-.json",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 365,
                        shared: true)))

                // 3. Business Log File (JSON, Information+, 60 days retention, 100MB roll)
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e =>
                        e.Level >= LogEventLevel.Information &&
                        (Matching.WithProperty(LogCategoryConstants.PropertyName, LogCategoryConstants.Business)(e) ||
                         Matching.FromSource("EnterpriseApi")(e)))
                    .WriteTo.Async(a => a.File(
                        formatter: businessFormatter,
                        path: "logs/business/business-.json",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 60,
                        fileSizeLimitBytes: 100 * 1024 * 1024,
                        rollOnFileSizeLimit: true)))

                // 4. Debug Log File (JSON, Debug/Verbose, 7 days retention, 2MB roll)
                .WriteTo.Logger(lc => lc
                    .Filter.ByIncludingOnly(e =>
                        e.Level <= LogEventLevel.Debug ||
                        Matching.WithProperty(LogCategoryConstants.PropertyName, LogCategoryConstants.Debug)(e))
                    .WriteTo.Async(a => a.File(
                        formatter: debugFormatter,
                        path: "logs/debug/debug-.json",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        fileSizeLimitBytes: 2 * 1024 * 1024,
                        rollOnFileSizeLimit: true)))
                .CreateLogger();

            builder.Logging.ClearProviders();
            builder.Host.UseSerilog(Log.Logger, dispose: true);

            builder.Services.AddScoped<IAuditLogger, AuditLogger>();
            return builder;
        }

        public static WebApplication UseEnterpriseLogging(this WebApplication app)
        {
            var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
            AppLoggerProvider.Initialize(loggerFactory);
            return app;
        }
    }
}
