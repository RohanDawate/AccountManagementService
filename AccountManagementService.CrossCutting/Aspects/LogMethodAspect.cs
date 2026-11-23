using AccountManagementService.CrossCutting.Constants;
using AccountManagementService.CrossCutting.Logging;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace AccountManagementService.CrossCutting.Aspects
{
    public class LogMethodAspect : OverrideMethodAspect
    {
        // 1. Synchronous Methods
        public override dynamic? OverrideMethod()
        {
            var logger = AppLoggerProvider.CreateLogger(meta.Target.Type.ToType());
            var methodName = $"{meta.Target.Type.Name}.{meta.Target.Method.Name}";
            var args = new Dictionary<string, object?>();
            CaptureParameters(args);

            using var scope = logger.BeginScope(new Dictionary<string, object>
            {
                [LogCategoryConstants.PropertyName] = LogCategoryConstants.Debug
            });

            logger.LogInformation("ENTER {Method} | Args: {@Args}", methodName, args);
            var sw = Stopwatch.StartNew();

            try
            {
                var result = meta.Proceed();
                sw.Stop();
                logger.LogInformation("EXIT {Method} | ElapsedMs: {ElapsedMs}", methodName, sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                logger.LogError(
                    ex,
                    "EXCEPTION in {Method} | ElapsedMs: {ElapsedMs} | Error: {Message}",
                    methodName,
                    sw.ElapsedMilliseconds,
                    ex.Message);
                throw;
            }
        }

        // 2. Asynchronous Methods (Task, Task<T>, ValueTask)
        public override async Task<dynamic?> OverrideAsyncMethod()
        {
            var logger = AppLoggerProvider.CreateLogger(meta.Target.Type.ToType());
            var methodName = $"{meta.Target.Type.Name}.{meta.Target.Method.Name}";
            var args = new Dictionary<string, object?>();
            CaptureParameters(args);

            using var scope = logger.BeginScope(new Dictionary<string, object>
            {
                [LogCategoryConstants.PropertyName] = LogCategoryConstants.Debug
            });

            logger.LogInformation("ENTER {Method} | Args: {@Args}", methodName, args);
            var sw = Stopwatch.StartNew();

            try
            {
                var result = await meta.ProceedAsync();
                sw.Stop();
                logger.LogInformation("EXIT {Method} | ElapsedMs: {ElapsedMs}", methodName, sw.ElapsedMilliseconds);
                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();
                logger.LogError(
                    ex,
                    "EXCEPTION in {Method} | ElapsedMs: {ElapsedMs} | Error: {Message}",
                    methodName,
                    sw.ElapsedMilliseconds,
                    ex.Message);
                throw;
            }
        }

        [Template]
        private static void CaptureParameters(Dictionary<string, object?> dict)
        {
            foreach (var param in meta.Target.Parameters)
            {
                if (param.RefKind != RefKind.Out)
                {
                    dict[param.Name] = param.Value;
                }
            }
        }
    }
}
