using AccountManagementService.CrossCutting.Attributes;

namespace AccountManagementService.CrossCutting.Models
{
    [NoLog]
    public class MethodLogEntry
    {
        public string EventType { get; init; } = string.Empty; // "ENTER", "EXIT", "EXCEPTION"
        public string ClassName { get; init; } = string.Empty;
        public string MethodName { get; init; } = string.Empty;
        public Dictionary<string, object?>? Args { get; init; }
        public object? Result { get; init; }
        public long? ElapsedMs { get; init; }
        public string? ErrorMessage { get; init; }
        public string? ExceptionType { get; init; }
    }
}
