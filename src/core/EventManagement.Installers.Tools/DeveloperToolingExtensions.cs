using EventManagement.Installers.Tools.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace EventManagement.Installers.Tools
{
    public static class DeveloperToolingExtensions
    {
        public static void UseDeveloperTools(this IApplicationBuilder app,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerTools(configuration);
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
        }
    }
}