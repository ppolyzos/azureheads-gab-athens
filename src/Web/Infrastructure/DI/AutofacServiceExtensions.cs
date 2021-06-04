using System;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventManagement.Web.Infrastructure.DI.Modules;
using EventManagement.Web.Infrastructure.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EventManagement.Web.Infrastructure.DI
{
    public static class AutofacServiceExtensions
    {
        public static IServiceProvider AddAutofacService(this IServiceCollection services, IContainer container)
        {
            var containerBuilder = new ContainerBuilder();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // don't try to fix container warning, DI won't work properly (try to run Integration Tests)
            containerBuilder.Register(c => container).As<IContainer>().SingleInstance();

            var runtimeAssemblies = PlatformUtils.GetAllAssemblies(Program.AppName).ToArray();

            containerBuilder.RegisterModule(new SettingsModule(runtimeAssemblies));
            containerBuilder.RegisterModule(new ServiceModule(runtimeAssemblies));
            containerBuilder.RegisterModule(new RepositoryModule(runtimeAssemblies));
            containerBuilder.RegisterModule(new SeedersModule(runtimeAssemblies));

            containerBuilder.Populate(services);

            container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }

        public static void UseAutofacService(this IApplicationBuilder app, IContainer container,
            IHostApplicationLifetime applicationLifetime)
        {
            if (container == null) return;

            applicationLifetime.ApplicationStarted.Register(container.Dispose);
        }
    }
}