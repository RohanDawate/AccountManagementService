
using AccountManagementService.API.Middleware;
using AccountManagementService.API.Services;
using AccountManagementService.Business;
using AccountManagementService.CrossCutting.Interfaces;
using AccountManagementService.CrossCutting.Logging;
using Microsoft.AspNetCore.HttpOverrides;
using Scalar.AspNetCore;

namespace AccountManagementService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Plug in Enterprise Logging (Configured inside CrossCutting/Logging/SerilogBootstrap.cs)
            builder.AddEnterpriseLogging();

            // 2. Enable HttpContextAccessor (Required for LogRecordContext & TraceContext across the HTTP request)
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ITraceContext, TraceContext>();

            // 3. Register Business & API Services
            builder.Services.AddBusinessServices();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // 4. Bind generic ILoggerFactory to Metalama's AppLoggerProvider
            app.UseEnterpriseLogging();

            // 5. Forwarded Headers (Resolves real Client IP & HTTPS scheme behind a Load Balancer)
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // 1. Serves the OpenAPI spec at: https://localhost:<port>/openapi/v1.json
                app.MapOpenApi();

                // 2. Serves Scalar UI at: https://localhost:<port>/scalar/v1
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("Account Management Service API")
                           .WithTheme(ScalarTheme.BluePlanet);
                });

                // 3. Serves classic Swagger UI at: https://localhost:<port>/swagger
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Account Management API v1");
                });
            }

            // 2. Now run Correlation & Trace Middleware for actual API requests
            app.UseMiddleware<CorrelationAndTraceMiddleware>();

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
