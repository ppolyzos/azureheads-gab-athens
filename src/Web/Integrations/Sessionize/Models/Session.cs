using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EventManagement.Web.Integrations.Sessionize.Models
{
    public class Session
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
        [JsonPropertyName("startsAt")] public DateTime StartsAt { get; set; }
        [JsonPropertyName("endsAt")] public DateTime EndsAt { get; set; }
        [JsonPropertyName("isServiceSession")] public bool IsServiceSession { get; set; }
        [JsonPropertyName("isPlenumSession")] public bool IsPlenumSession { get; set; }

        [JsonPropertyName("room")] public string Room { get; set; }
        
        [JsonPropertyName("speakers")]
        public IEnumerable<SessionItem<string>> Speakers { get; set; }
        [JsonPropertyName("categories")] public IEnumerable<SessionCategory> Categories { get; set; }
    }
}