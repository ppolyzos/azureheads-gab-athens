using EventManagement.Api.Core.Infrastructure.Filters;
using EventManagement.Api.Core.Infrastructure.Input.CommaSeparated;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Api.Core.Infrastructure.Extensions
{
    public static class MvcFiltersExtensions
    {
        public static void AddCoreFilters(this MvcOptions options)
        {
            // allow delimited collection query properties
            options.ValueProviderFactories.AddDelimitedValueProviderFactory(',', '|');
            
            options.Filters.Add(new ValidateModelStateFilter());

            // Setup caching
            options.CacheProfiles.Add("default", new CacheProfile
            {
                Duration = 60,
            });
            options.CacheProfiles.Add("never", new CacheProfile
            {
                NoStore = true,
                Location = ResponseCacheLocation.None
            });
        }
    }
}