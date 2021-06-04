using Microsoft.Extensions.DependencyInjection;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace EventManagement.Web.Infrastructure.Output
{
    public static class MvcJsonOptionsExtensions
    {
        public static IMvcBuilder AddCustomJsonOptions(this IMvcBuilder mvcBuilder)
        {
            mvcBuilder.AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = SerializerSettings.Default.PropertyNamingPolicy;
                foreach (var converter in SerializerSettings.Default.Converters)
                {
                    options.JsonSerializerOptions.Converters.Add(converter);
                }
            });

            return mvcBuilder;
        }

        public static string ToJson(this object obj)
        {
            return JsonSerializer.Serialize(obj, SerializerSettings.Default);
        }
    }
}