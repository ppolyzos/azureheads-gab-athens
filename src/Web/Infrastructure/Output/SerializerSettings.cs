using System.Text.Json;
using System.Text.Json.Serialization;
using EventManagement.Web.Utilities.Json.Converters;

namespace EventManagement.Web.Infrastructure.Output
{
    public static class SerializerSettings
    {
        public static JsonSerializerOptions Default { get; }

        static SerializerSettings()
        {
            var settings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonDateTimeConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };

            Default = settings;
        }
    }
}