using EventManagement.Web.Infrastructure.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace EventManagement.Web.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "EventManagement - Web API",
                    Version = "v1",
                    Description = "The EventManagement Service HTTP API"
                });

                options.OperationFilter<AuthorizeCheckOperationFilter>();
            });

            return services;
        }
    }
}