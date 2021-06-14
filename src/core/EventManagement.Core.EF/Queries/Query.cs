using System.Text.Json.Serialization;
using EventManagement.Core.Enumerations;

namespace EventManagement.Core.EF.Queries
{
    public abstract class Query
    {
        [JsonPropertyName("d")]
        public QueryDisposition Disposition { get; set; }

        protected Query()
        {
            Disposition = Disposition;
        }
    }
}