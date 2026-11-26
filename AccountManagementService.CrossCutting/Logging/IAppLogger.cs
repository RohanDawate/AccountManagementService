namespace AccountManagementService.CrossCutting.Logging
{
    public interface IAppLogger
    {
        // --- Business ---
        void BusinessInfo(string message, params object?[] args);
        void BusinessWarning(string message, params object?[] args);
        void BusinessWarning(Exception? ex, string message, params object?[] args);
        void BusinessError(string message, params object?[] args);
        void BusinessError(Exception? ex, string message, params object?[] args);
        void BusinessFatal(Exception? ex, string message, params object?[] args);

        //// --- Audit ---
        void AuditInfo(string message, params object?[] args);
        void AuditWarning(string message, params object?[] args);
        void AuditWarning(Exception? ex, string message, params object?[] args);
        void AuditError(string message, params object?[] args);
        void AuditError(Exception? ex, string message, params object?[] args);
        void AuditFatal(Exception? ex, string message, params object?[] args);

        //// --- Dev / Debug ---
        void DevDebug(string message, params object?[] args);
        void DevInfo(string message, params object?[] args);
        void DevWarning(string message, params object?[] args);
        void DevWarning(Exception? ex, string message, params object?[] args);
        void DevError(string message, params object?[] args);
        void DevError(Exception? ex, string message, params object?[] args);
        void DevFatal(Exception? ex, string message, params object?[] args);

    }

    public interface IAppLogger<out TCategoryName> : IAppLogger { }
}
