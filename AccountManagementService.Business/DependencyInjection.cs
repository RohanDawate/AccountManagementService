using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccountManagementService.Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Repositories
            //.AddScoped<IAccountRepository, AccountRepository>();

            // Services
            services.AddScoped<IAcc, Acc>();

            return services;
        }
    }
}
