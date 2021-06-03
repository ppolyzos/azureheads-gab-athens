using System.Text.Json.Serialization;

namespace EventManagement.Web.Integrations.Sessionize.Models
{
    public class Error
    {
        [JsonPropertyName("status")] public string Status { get; set; }

        [JsonPropertyName("code")] public string Code { get; set; }

        [JsonPropertyName("message")] public string Message { get; set; }
    }

    public class ErrorResponse
    {
        [JsonPropertyName("error")] public Error Error { get; set; }
    }
}