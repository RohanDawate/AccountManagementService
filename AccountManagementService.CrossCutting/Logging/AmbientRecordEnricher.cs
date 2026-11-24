using Serilog.Core;
using Serilog.Events;

namespace AccountManagementService.CrossCutting.Logging
{
    public sealed class AmbientRecordEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var context = RecordLogContextHolder.Current;
            if (context is null)
                return;

            if (context.RecIdentifier is not null)
            {
                logEvent.AddPropertyIfAbsent(
                    propertyFactory.CreateProperty("recidentifier", context.RecIdentifier));
            }

            if (context.RecValue is not null)
            {
                logEvent.AddPropertyIfAbsent(
                    propertyFactory.CreateProperty("recvalue", context.RecValue, destructureObjects: true));
            }
        }
    }
}
