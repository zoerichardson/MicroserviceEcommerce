using ECommerce.SharedLibrary.DependencyInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Application.Interfaces;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;

namespace ProductApi.Infrastructure.DependancyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            //add database connectivity
            //add authentiction scheme

            SharedServiceContainer.AddSharedServices<ProductDbContext>(services, config, config["MySerilog:Filename"]!);

            //create dependency injection
            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }


        public static IApplicationBuilder UseInfrastucturePolicy(this IApplicationBuilder app)
        {
            //register middleware such as:
            //Global Exception: handles external errors
            //Listen to Only Api Gateway: blocks all outsider calls
            SharedServiceContainer.UseSharedPolicies(app);

            return app;
        }
    }
}
