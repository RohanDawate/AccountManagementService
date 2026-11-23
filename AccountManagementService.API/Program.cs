
using AccountManagementService.Business;
using AccountManagementService.CrossCutting.Logging;
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

            // 2. Register Business & API Services
            builder.Services.AddBusinessServices();
            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // 3. Bind generic ILoggerFactory to Metalama's AppLoggerProvider
            app.UseEnterpriseLogging();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // 1. Serves the OpenAPI spec at: https://localhost:<port>/openapi/v1.json
                app.MapOpenApi();

                // 2. Serves Scalar UI at: https://localhost:<port>/scalar/v1
                app.MapScalarApiReference(options =>
                {
                    options.WithTitle("Account Management Service API")
                           .WithTheme(ScalarTheme.Solarized);
                });

                // 3. Serves classic Swagger UI at: https://localhost:<port>/swagger
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "Account Management API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
