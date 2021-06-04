using System.Text.Json.Serialization;

namespace EventManagement.Web.Integrations.Sessionize.Models
{
    public class ErrorResponse
    {
        [JsonPropertyName("error")] public Error Error { get; set; }
    }
}