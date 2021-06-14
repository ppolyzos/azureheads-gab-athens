using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EventManagement.Core.Enumerations;
using EventManagement.Core.Utilities.Json.Converters;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Core.EF.Queries.Base
{
    public abstract class QueryBase : Query, IObjectQuery
    {
        private string _cacheKey;
        private JsonSerializerOptions Settings { get; }

        public abstract object Key { get; set; }

        [JsonIgnore] public string GroupKey { get; set; }

        protected QueryBase()
        {
            Disposition = QueryDisposition.Required;

            Settings = new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                Converters = { new JsonDateTimeConverter() }
            };
        }

        public abstract IQueryable<TModel> BuildQuery<TModel>(DbContext dbContext)
            where TModel : class;

        [JsonIgnore]
        public string CacheKey
        {
            get
            {
                var s = new StringBuilder(string.IsNullOrEmpty(GroupKey) ? string.Empty : $"{GroupKey}_");
                s.Append(string.IsNullOrEmpty(_cacheKey)
                    ? JsonSerializer.Serialize(this, Settings).Replace("\"", "")
                    : _cacheKey);

                return s.Replace("\"", "").ToString();
            }
            set => _cacheKey = value;
        }

        /// <summary>
        /// The number of items to skip before returning the results
        /// </summary>
        [JsonPropertyName("skip")]
        public int? NumberOfItemsToSkip { get; set; }

        /// <summary>
        /// The number of items to return
        /// </summary>
        [JsonPropertyName("take")]
        public int? NumberOfItemsToReturn { get; set; }
    }
}