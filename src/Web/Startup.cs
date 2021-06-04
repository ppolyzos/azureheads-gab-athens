using System;
using Autofac;
using EventManagement.Web.Configuration.Extensions;
using EventManagement.Web.Infrastructure.DI;
using EventManagement.Web.Installers.Core;
using EventManagement.Web.Installers.Tools;
using EventManagement.Web.Installers.Tools.HealthChecks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.SetupConfiguration(Configuration);
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

            app.UseDeveloperTools(env, Configuration);

            app.UseRouting();
            app.UseCors("default");
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapAppHealthChecks();
            });
        }
    }
}