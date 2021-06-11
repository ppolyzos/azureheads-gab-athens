using System.Collections.Generic;
using System.Reflection;
using Autofac;

namespace EventManagement.Api.Core.Infrastructure.DI.Modules
{
    public class ServiceModule : Autofac.Module
    {
        private readonly IEnumerable<Assembly> _assemblies;

        public ServiceModule(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var assembly in _assemblies)
            {
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("Service"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }
        }
    }
}
