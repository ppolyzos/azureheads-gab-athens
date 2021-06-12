using EventManagement.Installers.Tools.Infrastructure.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace EventManagement.Installers.Tools.Swagger
{
    public static class SwaggerExtensions
    {
        public static IApplicationBuilder UseSwaggerTools(this IApplicationBuilder app,
            IConfiguration configuration)
        {
            var pathBase = configuration["PATH_BASE"];

            app.UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(
                        $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json",
                        "Event Management V1");
                    options.OAuthClientId("eventmngtswaggerui");
                    options.OAuthAppName("EventManagement.Web.API Swagger UI");
                });

            return app;
        }
        
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