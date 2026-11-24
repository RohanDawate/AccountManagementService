namespace AccountManagementService.CrossCutting.Logging
{
    public sealed record RecordLogContext(string? RecIdentifier, object? RecValue);

    public static class RecordLogContextHolder
    {
        private static readonly AsyncLocal<RecordLogContext?> _current = new();

        public static RecordLogContext? Current
        {
            get => _current.Value;
            set => _current.Value = value;
        }
    }

    public static class RecordLog
    {
        public static void Set(string recIdentifier, object? recValue)
        {
            RecordLogContextHolder.Current = new RecordLogContext(recIdentifier, recValue);
        }

        public static void Clear()
        {
            RecordLogContextHolder.Current = null;
        }
    }
}
