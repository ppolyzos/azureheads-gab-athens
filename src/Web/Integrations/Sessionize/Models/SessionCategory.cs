using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EventManagement.Web.Integrations.Sessionize.Models
{
    public class SessionCategory
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("categoryItems")] public IEnumerable<SessionItem<int>> CategoryItems { get; set; }
    }
}