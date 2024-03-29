﻿using System;
using System.Linq;
using Autofac;
using EventManagement.Api.Core.Infrastructure.DI;
using EventManagement.Api.Core.Installers;
using EventManagement.Api.Core.Utilities;
using EventManagement.Installers.Tools;
using EventManagement.Installers.Tools.HealthChecks;
using EventManagement.Web.Configuration.Extensions;
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
            services.InstallServicesIn(Configuration,
                PlatformUtils.GetAllAssemblies("EventManagement.Installers.Tools").Concat(
                    PlatformUtils.GetAssembliesBasedOn<Startup>()));

            var runtimeAssemblies = PlatformUtils.GetAllAssemblies(Program.AppName).ToArray();
            return services.AddAutofacService(Container, runtimeAssemblies);
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

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("default");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
                endpoints.MapAppHealthChecks();
            });
        }
    }
}