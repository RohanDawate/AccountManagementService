using AccountManagementService.CrossCutting.Attributes;
using Microsoft.AspNetCore.Http;
using System.Collections;
using System.Data;
using System.IO.Pipelines;

namespace AccountManagementService.CrossCutting.Logging
{
    [NoLog]
    public static class LogArgumentSanitizer
    {

        public static bool ShouldLogValue(object? value)
        {
            if (value is null or string)
                return true;

            // 1. Block Collections, DataTables, and CancellationTokens
            if (value is DataTable or DataSet or IEnumerable or CancellationToken)
                return false;

            // 2. Block ASP.NET Core HTTP / Stream / DI objects (prevents massive graph serialization & crashes!)
            if (value is HttpContext or HttpRequest or HttpResponse or Stream or PipeReader or PipeWriter or IServiceProvider or Delegate)
                return false;

            // 3. Block any Microsoft.AspNetCore.* or System.Reflection.* framework types
            var ns = value.GetType().Namespace;
            if (ns != null && (ns.StartsWith("Microsoft.AspNetCore") || ns.StartsWith("System.Reflection")))
                return false;

            return true;
        }

        public static Dictionary<string, object?>? NullIfEmpty(Dictionary<string, object?> args)
            => args.Count > 0 ? args : null;

        public static Dictionary<string, object?>? Sanitize(Dictionary<string, object?>? rawArgs)
        {
            if (rawArgs == null || rawArgs.Count == 0)
                return null;

            var sanitized = new Dictionary<string, object?>();

            foreach (var (key, value) in rawArgs)
            {
                if (ShouldLogValue(value))
                {
                    sanitized[key] = value;
                }
            }

            return sanitized.Count > 0 ? sanitized : null;
        }

        public static object? SanitizeResult(object? result)
        {
            if (result == null || !ShouldLogValue(result))
                return null;

            return result;
        }

    }


}
