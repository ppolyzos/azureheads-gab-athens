using System.Text.Json.Serialization;

namespace EventManagement.Web.Integrations.Sessionize.Models
{
    public class Link
    {
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("url")] public string Url { get; set; }
        [JsonPropertyName("linkType")] public string LinkType { get; set; }
    }
}