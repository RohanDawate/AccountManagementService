namespace AccountManagementService.CrossCutting.Interfaces
{
    public interface ITraceContext
    {
        string CorrelationId { get; }
        string TraceId { get; }
    }
}
