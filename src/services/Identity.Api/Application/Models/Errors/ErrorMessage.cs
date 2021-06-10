using System.Text.Json.Serialization;

namespace Identity.Api.Application.Models.Errors
{
    public class ErrorMessage
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("msg")]
        public string Message { get; set; }
        [JsonPropertyName("req")]
        public string RequestId { get; set; }
    }
}
