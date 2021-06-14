using System.Text.Json.Serialization;
using EventManagement.Core.Enumerations;

namespace EventManagement.Core.EF.Queries
{
    public class QueryOrderInfo
    {
        [JsonPropertyName("o")]
        public string OrderByProperty { get; set; }

        [JsonPropertyName("d")]
        public QueryOrderDirection OrderDirection { get; set; }

        public QueryOrderInfo(string orderByProperty)
        {
            OrderByProperty = orderByProperty;
            OrderDirection = QueryOrderDirection.Ascending;
        }

        public QueryOrderInfo(string orderByProperty, QueryOrderDirection orderDirection)
            : this(orderByProperty)
        {
            OrderDirection = orderDirection;
        }
    }
}