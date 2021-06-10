using System.Collections.Generic;
using System.Reflection;
using Autofac;

namespace Identity.Api.Infrastructure.DI.Modules
{
    public class SeedersModule : Autofac.Module
    {
        private readonly IEnumerable<Assembly> _assemblies;

        public SeedersModule(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }

        protected override void Load(ContainerBuilder builder)
        {
            foreach (var assembly in _assemblies)
            {
                builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("DataSeeder"))
                    .AsSelf()
                    .InstancePerLifetimeScope();
            }
        }
    }
}
