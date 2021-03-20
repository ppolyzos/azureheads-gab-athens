using System.Text.Json;
using System.Text.Json.Serialization;
using gab_athens.Utilities.Json.Converters;

namespace gab_athens.Infrastructure.Output
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