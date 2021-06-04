using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EventManagement.Web.Integrations.Sessionize.Models
{
    public class GroupSession
    {
        [JsonPropertyName("groupId")] public string GroupId { get; set; }
        [JsonPropertyName("groupName")] public string GroupName { get; set; }

        [JsonPropertyName("sessions")] public IEnumerable<Session> Sessions { get; set; }
    }
}