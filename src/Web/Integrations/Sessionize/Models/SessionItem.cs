using System.Text.Json.Serialization;

namespace EventManagement.Web.Integrations.Sessionize.Models
{
    public class SessionItem<T>
    {
        [JsonPropertyName("id")] public T Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
    }
}