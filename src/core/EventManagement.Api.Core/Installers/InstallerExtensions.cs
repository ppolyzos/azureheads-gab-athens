using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventManagement.Api.Core.Installers
{
    public static class InstallerExtensions
    {
        public static void InstallServicesInAssembly<T>(this IServiceCollection services, IConfiguration configuration)
        {
            services.InstallServicesIn(configuration, new[] { typeof(T).Assembly });
        }

        public static void InstallServicesIn(this IServiceCollection services,
            IConfiguration configuration, IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var installers = assembly.ExportedTypes
                    .Where(c => typeof(IInstaller).IsAssignableFrom(c)
                                && !c.IsInterface
                                && !c.IsAbstract)
                    .Select(Activator.CreateInstance)
                    .Cast<IInstaller>()
                    .ToList();

                installers.ForEach(installer => installer.InstallServices(services, configuration));
            }
        }
    }
}