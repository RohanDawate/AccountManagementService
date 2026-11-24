using AccountManagementService.CrossCutting.Constants;
using AccountManagementService.CrossCutting.Logging;
using AccountManagementService.CrossCutting.Models;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Microsoft.Extensions.Logging;
using System.Diagnostics;


namespace AccountManagementService.CrossCutting.Aspects
{
    public class LogMethodAspect : OverrideMethodAspect
    {
        // Tells Metalama at compile-time: skip weaving completely if this is a Middleware or InvokeAsync
        public override void BuildAspect(IAspectBuilder<IMethod> builder)
        {
            var method = builder.Target;
            if (method.DeclaringType.Name.EndsWith("Middleware") ||
                method.Name is "Invoke" or "InvokeAsync")
            {
                builder.SkipAspect();
                return;
            }

            base.BuildAspect(builder);
        }

        // 1. Synchronous Methods
        public override dynamic? OverrideMethod()
        {
            var logger = AppLoggerProvider.CreateLogger(meta.Target.Type.ToType());
            var className = meta.Target.Type.Name;
            var methodName = meta.Target.Method.Name;

            var rawArgs = new Dictionary<string, object?>();
            CaptureParameters(rawArgs);
            var sanitizedArgs = LogArgumentSanitizer.Sanitize(rawArgs);

            using var scope = logger.BeginScope(new Dictionary<string, object>
            {
                [LogCategoryConstants.PropertyName] = LogCategoryConstants.Debug
            });

            logger.LogInformation("{@LogEntry}", new MethodLogEntry
            {
                EventType = "ENTER",
                ClassName = className,
                MethodName = methodName,
                Args = sanitizedArgs
            });

            var sw = Stopwatch.StartNew();

            try
            {
                var result = meta.Proceed();
                sw.Stop();

                object? sanitizedResult = meta.Target.Method.ReturnType.SpecialType == SpecialType.Void
                ? null
                : LogArgumentSanitizer.SanitizeResult(result);

                logger.LogInformation("{@LogEntry}", new MethodLogEntry
                {
                    EventType = "EXIT",
                    ClassName = className,
                    MethodName = methodName,
                    Result = sanitizedResult,
                    ElapsedMs = sw.ElapsedMilliseconds
                });

                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();

                logger.LogError(ex, "{@LogEntry}", new MethodLogEntry
                {
                    EventType = "EXCEPTION",
                    ClassName = className,
                    MethodName = methodName,
                    Args = sanitizedArgs,
                    ElapsedMs = sw.ElapsedMilliseconds,
                    ExceptionType = ex.GetType().Name,
                    ErrorMessage = ex.Message
                });

                throw;
            }
        }

        // 2. Asynchronous Methods (Task, Task<T>, ValueTask)
        public override async Task<dynamic?> OverrideAsyncMethod()
        {
            var logger = AppLoggerProvider.CreateLogger(meta.Target.Type.ToType());
            var className = meta.Target.Type.Name;
            var methodName = meta.Target.Method.Name;

            var rawArgs = new Dictionary<string, object?>();
            CaptureParameters(rawArgs);
            var sanitizedArgs = LogArgumentSanitizer.Sanitize(rawArgs);

            using var scope = logger.BeginScope(new Dictionary<string, object>
            {
                [LogCategoryConstants.PropertyName] = LogCategoryConstants.Debug
            });

            logger.LogInformation("{@LogEntry}", new MethodLogEntry
            {
                EventType = "ENTER",
                ClassName = className,
                MethodName = methodName,
                Args = sanitizedArgs
            });

            var sw = Stopwatch.StartNew();

            try
            {
                var result = await meta.ProceedAsync();
                sw.Stop();

                // Check if async method returns non-generic Task/ValueTask (no result payload)
                var isVoidAsync = meta.Target.Method.ReturnType is INamedType { TypeArguments.Count: 0 };
                object? sanitizedResult = isVoidAsync
                    ? null
                    : LogArgumentSanitizer.SanitizeResult(result);

                logger.LogInformation("{@LogEntry}", new MethodLogEntry
                {
                    EventType = "EXIT",
                    ClassName = className,
                    MethodName = methodName,
                    Result = sanitizedResult,
                    ElapsedMs = sw.ElapsedMilliseconds
                });

                logger.LogInformation(
                "EXIT {Method} | ElapsedMs: {ElapsedMs} | Result: {@Result}",
                methodName,
                sw.ElapsedMilliseconds,
                sanitizedResult);

                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();

                logger.LogError(ex, "{@LogEntry}", new MethodLogEntry
                {
                    EventType = "EXCEPTION",
                    ClassName = className,
                    MethodName = methodName,
                    Args = sanitizedArgs,
                    ElapsedMs = sw.ElapsedMilliseconds,
                    ExceptionType = ex.GetType().Name,
                    ErrorMessage = ex.Message
                });

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
