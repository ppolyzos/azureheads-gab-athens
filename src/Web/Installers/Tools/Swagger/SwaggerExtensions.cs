using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace EventManagement.Web.Installers.Tools.Swagger
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
    }
}