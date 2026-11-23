using AccountManagementService.CrossCutting.Attributes;
using AccountManagementService.CrossCutting.Constants;
using Microsoft.Extensions.Logging;

namespace AccountManagementService.CrossCutting.Logging
{
    public interface IAuditLogger
    {
        void LogAudit(
            string action,
            string entityName,
            string entityId,
            string userId = "SYSTEM",
            object? details = null,
            bool isSuccess = true);
    }

    [NoLog]
    public class AuditLogger : IAuditLogger
    {
        private readonly ILogger<AuditLogger> _logger;

        public AuditLogger(ILogger<AuditLogger> logger)
        {
            _logger = logger;
        }

        public void LogAudit(
            string action,
            string entityName,
            string entityId,
            string userId = "SYSTEM",
            object? details = null,
            bool isSuccess = true)
        {
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                [LogCategoryConstants.PropertyName] = LogCategoryConstants.Audit
            }))
            {
                _logger.LogInformation(
                    "AUDIT | Action: {AuditAction} | Entity: {EntityName} ({EntityId}) | Status: {AuditStatus} | User: {UserId} | Details: {@AuditDetails}",
                    action,
                    entityName,
                    entityId,
                    isSuccess ? "SUCCESS" : "FAILURE",
                    userId,
                    details);
            }
        }
    }
}