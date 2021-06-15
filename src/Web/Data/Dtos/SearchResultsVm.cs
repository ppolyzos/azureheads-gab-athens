using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EventManagement.Web.Data.Dtos
{
    public class SearchResultsDto<TDto>
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("items")]
        public IEnumerable<TDto> Results { get; set; }
    }
}
