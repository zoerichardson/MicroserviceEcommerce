using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Repositories;
using ECommerce.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            //add db connectivity
            //JWT add authentication scheme
            SharedServiceContainer.AddSharedServices<AuthenticationDbContext>(services, config, config["MySerilog:Filename"]!);

            //create DI
            services.AddScoped<IUser, UserRepository>();



            return services;
        }

        public static IApplicationBuilder UserInfrastructrePolicy(this IApplicationBuilder app)
        {
            //register middleware such as:
            //Global Exception: Handle external errors
            //Listen Only To Api Gateway : block all outsiders call
            SharedServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}

