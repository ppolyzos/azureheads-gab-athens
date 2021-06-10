using System;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Api.Infrastructure.Extensions.Auth
{
    public static class AuthorizationServiceExtensions
    {
        public static IServiceCollection AddDomainAuthorization<T>(this IServiceCollection services)
            where T : ICustomPolicyBuilder
        {
            if (!(Activator.CreateInstance<T>() is ICustomPolicyBuilder instance))
                throw new ArgumentException("Value of T should be of type ICustomPolicyBuilder");

            services.AddAuthorization(o =>
            {
                foreach (var (key, value) in instance.GetPolicies())
                {
                    o.AddPolicy(key, value);
                }
            });

            return services;
        }
    }
}
