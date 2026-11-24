using AccountManagementService.CrossCutting.Attributes;
using AccountManagementService.CrossCutting.Constants;
using Serilog.Events;
using Serilog.Formatting;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AccountManagementService.CrossCutting.Logging
{
    [NoLog]
    public class FlatJsonFormatter : ITextFormatter
    {
        private static readonly TimeZoneInfo IstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");
        private static readonly string ServerNode = Environment.MachineName;
        private static readonly string ServerIp = ResolveServerIpAddress();

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false // Guarantees single-line JSON (NDJSON)
        };

        private static string ResolveServerIpAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                var ip = host.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(a));
                return ip?.ToString() ?? "127.0.0.1";
            }
            catch
            {
                return "unknown";
            }
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            // Convert UTC timestamp to IST and format as dd-MM-yyyy HH:mm:ss.fff
            var istTime = TimeZoneInfo.ConvertTime(logEvent.Timestamp, IstTimeZone);
            var formattedTimestamp = istTime.ToString("dd-MM-yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);

            var root = new Dictionary<string, object?>
            {
                ["timestamp"] = formattedTimestamp,
                ["level"] = logEvent.Level.ToString(),
                ["category"] = GetScalarString(logEvent, LogCategoryConstants.PropertyName) ?? "Business",
                ["server"] = ServerNode,
                ["serverIp"] = ServerIp
            };

            // Optional ambient scope properties (CorrelationId, TraceId, etc.)
            AddIfPresent(logEvent, root, "correlationId", "CorrelationId");
            AddIfPresent(logEvent, root, "traceId", "TraceId");
            AddIfPresent(logEvent, root, "server_ip", "ServerIp");
            AddIfPresent(logEvent, root, "server_name", "server");

            // Record identifier & value (e.g. recidentifier = "AccountNo", recvalue = "123456")
            AddRecordContext(logEvent, root);

            // 1. Flatten BusinessLogEntry ("LogEntry") -> keep Args & Result as nested objects
            if (logEvent.Properties.TryGetValue("LogEntry", out var logEntryProp) && logEntryProp is StructureValue bizStruct)
            {
                FlattenStructure(bizStruct, root);
            }
            // 2. Flatten AuditLogEntry ("AuditEntry") -> keep Details as nested object
            else if (logEvent.Properties.TryGetValue("AuditEntry", out var auditProp) && auditProp is StructureValue auditStruct)
            {
                FlattenStructure(auditStruct, root);
            }
            // 3. Flatten DebugLogEntry ("DebugEntry") -> keep Data as nested object
            else if (logEvent.Properties.TryGetValue("DebugEntry", out var debugProp) && debugProp is StructureValue debugStruct)
            {
                FlattenStructure(debugStruct, root);
            }
            // 4. Fallback for standard framework/unstructured ILogger calls
            else
            {
                root["source"] = GetScalarString(logEvent, "SourceContext");
                root["message"] = logEvent.RenderMessage();
            }

            if (logEvent.Exception != null)
            {
                root["exception"] = logEvent.Exception.ToString();
            }

            output.WriteLine(JsonSerializer.Serialize(root, JsonOptions));
        }

        private static void AddRecordContext(LogEvent logEvent, Dictionary<string, object?> root)
        {
            // Prefer enriched properties if present, otherwise read directly from ambient context
            string? recIdentifier = GetScalarString(logEvent, "recidentifier")
                ?? RecordLogContextHolder.Current?.RecIdentifier;

            object? recValue = logEvent.Properties.TryGetValue("recvalue", out var propVal)
                ? ConvertPropertyValue(propVal)
                : RecordLogContextHolder.Current?.RecValue;

            if (!string.IsNullOrWhiteSpace(recIdentifier))
            {
                root["recidentifier"] = recIdentifier;
            }

            if (recValue is not null)
            {
                root["recvalue"] = recValue;
            }
        }

        private static void FlattenStructure(StructureValue structure, Dictionary<string, object?> target)
        {
            foreach (var prop in structure.Properties)
            {
                // Convert PascalCase property names (EventType, Args, Result) to camelCase (eventType, args, result)
                var camelKey = char.ToLowerInvariant(prop.Name[0]) + prop.Name[1..];

                // Convert LogEventPropertyValue to native CLR objects/dictionaries WITHOUT flattening nested objects
                target[camelKey] = ConvertPropertyValue(prop.Value);
            }
        }

        private static object? ConvertPropertyValue(LogEventPropertyValue value)
        {
            return value switch
            {
                ScalarValue scalar => scalar.Value,

                // Nested objects (Args, Result, Customer DTO, Details, Data) stay intact as nested JSON objects!
                StructureValue structure => structure.Properties
                    .Where(p => p.Name != "$type") // Strip Serilog's "$type" noise
                    .ToDictionary(
                        p => char.ToLowerInvariant(p.Name[0]) + p.Name[1..],
                        p => ConvertPropertyValue(p.Value)),

                DictionaryValue dict => dict.Elements.ToDictionary(
                    kvp => kvp.Key.Value?.ToString() ?? string.Empty,
                    kvp => ConvertPropertyValue(kvp.Value)),

                SequenceValue seq => seq.Elements.Select(ConvertPropertyValue).ToList(),

                _ => value.ToString()
            };
        }

        private static string? GetScalarString(LogEvent logEvent, string propertyName)
        {
            if (logEvent.Properties.TryGetValue(propertyName, out var val) && val is ScalarValue scalar)
                return scalar.Value?.ToString();

            return null;
        }

        private static void AddIfPresent(LogEvent logEvent, Dictionary<string, object?> target, string jsonKey, string propName)
        {
            if (logEvent.Properties.TryGetValue(propName, out var val) && val is ScalarValue scalar && scalar.Value != null)
                target[jsonKey] = scalar.Value;
        }
    }
}
