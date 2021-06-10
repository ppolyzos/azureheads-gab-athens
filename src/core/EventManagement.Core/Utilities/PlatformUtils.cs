using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace EventManagement.Core.Utilities
{
    public static class PlatformUtils
    {
        public static IEnumerable<Assembly> GetAllAssemblies(string startsWith)
        {
            var platform = Environment.OSVersion.Platform.ToString();
            var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

            return runtimeAssemblyNames
                .Where(a => a.Name != null && a.Name.StartsWith(startsWith))
                .Select(Assembly.Load);
        }

        public static IEnumerable<Type> GetAllTypesOf<T>(string startsWith)
        {
            return GetAllTypes(startsWith)
                .Where(t => typeof(T).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
        }

        private static IEnumerable<Type> GetAllTypes(string startsWith)
        {
            return GetAllAssemblies(startsWith)
                .SelectMany(a => a.ExportedTypes);
        }
    }
}
