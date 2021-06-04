using System;
using System.Collections.Generic;
using Autofac;
using EventManagement.Web.Configuration.Extensions;
using EventManagement.Web.Extensions;
using EventManagement.Web.Infrastructure.DI;
using EventManagement.Web.Installers.Core;
using EventManagement.Web.Integrations.Sessionize;
using EventManagement.Web.Services;
using EventManagement.Web.Services.Storage;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace EventManagement.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IContainer Container { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy());

            services.AddSwagger(Configuration);

            services.SetupConfiguration(Configuration);
            services.AddApplicationInsightsTelemetry();
            
            services.AddSingleton<IEventDataStorageService, EventDataStorageService>();

            services.InstallServicesInAssembly(Configuration);
            
            return services.AddAutofacService(Container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var pathBase = Configuration["PATH_BASE"];
            if (!string.IsNullOrEmpty(pathBase))
            {
                app.UsePathBase(pathBase);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger()
                    .UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint(
                            $"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json",
                            "Event Management V1");
                        options.OAuthClientId("eventmngtswaggerui");
                        options.OAuthAppName("EventManagement.Web.API Swagger UI");
                    });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseRouting();
            app.UseCors("default");
            app.UseStaticFiles();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });
        }
    }
}