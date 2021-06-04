using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EventManagement.Web.Data.ViewModels
{
    public class SearchResultsVm<TVm>
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("items")]
        public IEnumerable<TVm> Results { get; set; }
    }
}
