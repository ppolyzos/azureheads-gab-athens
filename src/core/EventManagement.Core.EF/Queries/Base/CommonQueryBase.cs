using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Serialization;
using EventManagement.Core.EF.Extensions;
using EventManagement.Core.Enumerations;

namespace EventManagement.Core.EF.Queries.Base
{
    public abstract class CommonQueryBase : QueryBase
    {
        #region QueryCollections

        protected List<Status> Statuses => Collection<Status>(CommonCollection.Status);

        protected List<Visibility> Visibilities => Collection<Visibility>(CommonCollection.Visibility);

        protected List<object> IncludedObjectIds => Collection<object>(CommonCollection.ObjectsIncluded);

        protected List<object> ExcludedObjectIds => Collection<object>(CommonCollection.ObjectsExcluded);

        protected List<TextFieldQuery> TextFieldQueries => Collection<TextFieldQuery>(CommonCollection.TextFields);

        private List<QueryOrderInfo> OrderInfos => Collection<QueryOrderInfo>(CommonCollection.QueryOrderInfo);
        private List<QueryOrderInfo> OrderInfosSpecials => Collection<QueryOrderInfo>(CommonCollection.QueryOrderInfoSpecials);

        protected List<CommonQueryBase> SubQueries => Collection<CommonQueryBase>(CommonCollection.SubQueries);

        /// <summary>
        /// Host multiple object collections based on needs of each query
        /// </summary>
        [JsonPropertyName("c")]
        public Dictionary<int, object> QueryCollections { get; }

        #endregion

        #region Properties
        private object _key;
        public override object Key
        {
            get => _key;
            set
            {
                _key = value;
                IncludeObjects(_key);
                PresentableResultsOnly = false;
            }
        }

        /// <summary>
        /// When true, returns distinct results.
        /// The default value is false, which means that duplicate objects may be returned.
        /// </summary>
        [JsonPropertyName("dist")]
        public bool DistinctResultsOnly { get; set; }

        /// <summary>
        /// When true, returns only presentable objects.
        /// The default value is true, which means that only presentable objects will be returned from this query.
        /// </summary>
        [JsonPropertyName("pres")]
        public bool PresentableResultsOnly { get; set; }

        /// <summary>
        /// Specifies the text search mode that will be used for searching inside textual data.
        /// The default value is Simple
        /// </summary>
        [JsonIgnore]
        public TextSearchMode TextSearchMode { get; set; }

        /// <summary>
        /// Specifies the DateTime results should have been created from
        /// </summary>
        [JsonPropertyName("insf")]
        public DateTime? InsertionDateFrom { get; set; }

        /// <summary>
        /// Specifies the DateTime till results should have been created
        /// </summary>
        [JsonPropertyName("inst")]
        public DateTime? InsertionDateTo { get; set; }

        /// <summary>
        /// Specifies the DateTime results should have been modified from
        /// </summary>
        [JsonPropertyName("modf")]
        public DateTime? ModifiedDateFrom { get; set; }

        /// <summary>
        /// Specifies the DateTime till results should have been modified
        /// </summary>
        [JsonPropertyName("modt")]
        public DateTime? ModifiedDateTo { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a new instance of <see cref="CommonQueryBase"/>
        /// </summary>
        protected CommonQueryBase()
        {
            PresentableResultsOnly = false;

            QueryCollections = new Dictionary<int, object>();
        }
        #endregion

        #region Collection Handling
        protected void AddToCollection<T>(Enum name, T item)
        {
            Collection<T>(name)?.Add(item);
        }

        protected void AddToCollection<T>(Enum name, params T[] items) => AddToCollection(name, items as IEnumerable<T>);

        protected void AddToCollection<T>(Enum name, IEnumerable<T> items)
        {
            if (items == null) return;

            var itemsArray = items as T[] ?? items.ToArray();
            if (!itemsArray.Any()) return;

            Collection<T>(name)?.AddRange(itemsArray);
        }
        
        protected void SetupCollectionIfNotExists<T>(Enum name)
        {
            var key = Convert.ToInt32(name);
            if (QueryCollections.ContainsKey(key)) return;

            QueryCollections.Add(key, new List<T>());
        }

        protected List<T> Collection<T>(Enum name)
        {
            var key = Convert.ToInt32(name);
            if (!QueryCollections.ContainsKey(key))
            {
                SetupCollectionIfNotExists<T>(name);
            }
            return QueryCollections[key] as List<T>;
        }
        #endregion

        #region Common
        protected IQueryable<TModel> IncludeCommon<TModel>(IQueryable<TModel> query, string defaultOrderInfo = "Id")
        {
            query = query.With(DistinctResultsOnly, query.Distinct());

            if (OrderInfos.Any())
            {
                foreach (var orderInfo in OrderInfos.Reverse<QueryOrderInfo>())
                {
                    if (!orderInfo.OrderByProperty.Equals("distance", StringComparison.InvariantCultureIgnoreCase))
                    {
                        query = query.BuildOrder(orderInfo);
                    }
                }
            }
            else if (NumberOfItemsToSkip != null || NumberOfItemsToReturn != null)
            {
                query = query.BuildOrder(new QueryOrderInfo(defaultOrderInfo));
            }

            query = query.With(NumberOfItemsToSkip.HasValue, query.Skip(NumberOfItemsToSkip.GetValueOrDefault()));
            query = query.With(NumberOfItemsToReturn.HasValue, query.Take(NumberOfItemsToReturn.GetValueOrDefault()));

            return query;
        }
        #endregion

        #region Include/Exclude Objects

        public void IncludeObjects<T>(params T[] objectIds) => AddToCollection(CommonCollection.ObjectsIncluded, objectIds?.Select(o => (object) o));
        public void IncludeObjects(params int[] objectIds) => AddToCollection(CommonCollection.ObjectsIncluded, objectIds);
        public void IncludeObjects(params string[] objectIds) => AddToCollection(CommonCollection.ObjectsIncluded, objectIds);
        
        public void ExcludeObjects<T>(params T[] objectIds) => AddToCollection(CommonCollection.ObjectsExcluded, objectIds?.Select(o => (object) o));
        public void ExcludeObjects(params int[] objectIds) => AddToCollection(CommonCollection.ObjectsExcluded, objectIds);
        public void ExcludeObjects(params string[] objectIds) => AddToCollection(CommonCollection.ObjectsExcluded, objectIds);

        #endregion

        #region Status / Visibility

        public void AddStatus(params Status[] statuses)
        {
            PresentableResultsOnly = false;
            AddToCollection(CommonCollection.Status, statuses);
        }


        public void AddVisibility(params Visibility[] visibilities) => AddToCollection(CommonCollection.Visibility, visibilities);
        #endregion

        #region SubQueries
        public void AddSubQuery(CommonQueryBase query, QueryDisposition disposition)
        {
            query.Disposition = disposition;
            SubQueries.Add(query);
        }
        #endregion

        #region Utility
        private bool HasProperty<T>(string propertyName) => !string.IsNullOrEmpty(propertyName) && typeof(T).GetProperty(propertyName) != null;

        private static string GetPropertyName<T, TResult>(Expression<Func<T, TResult>> fieldMapping)
        {
            return GetProperty(fieldMapping)?.Name;
        }

        private static PropertyInfo GetProperty<T, TResult>(Expression<Func<T, TResult>> fieldMapping)
        {
            var member = fieldMapping.Body as MemberExpression;
            return member?.Member as PropertyInfo;
        }
        #endregion

        #region TextField Operations
        /// <summary>
        /// Create text field queries
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="op"></param>
        /// <param name="value">Null or empty values automatically ignored</param>
        /// <param name="disposition"></param>
        private void AddTextFieldQuery(string fieldName, TextFieldValueOperator op, string value, QueryDisposition disposition)
        {
            if (string.IsNullOrEmpty(value)) return;

            TextFieldQueries.Add(new TextFieldQuery
            {
                FieldName = fieldName,
                Value = value,
                Operator = op,
                Disposition = disposition
            });
        }

        protected void AddTextFieldExpressionQuery<TModel, TResult>(Expression<Func<TModel, TResult>> fieldMapping, TextFieldValueOperator op, string value,
            QueryDisposition disposition = QueryDisposition.Optional)
        {
            var property = GetProperty(fieldMapping);
            if (property == null)
                throw new ArgumentNullException($"{nameof(fieldMapping)} can not be mapped to any model properties");

            if (property.PropertyType != typeof(string)) 
                throw new ArgumentException($"{nameof(fieldMapping)} is only supported for string properties");

            AddTextFieldQuery(property.Name, op, value, disposition);
        }

        [Obsolete("has been altered to a more generic expression field name")]
        public void AddTextFieldQuery<T>(T fieldName, TextFieldValueOperator op, string value, QueryDisposition disposition = QueryDisposition.Optional) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            AddTextFieldQuery(fieldName.ToString(CultureInfo.InvariantCulture), op, value, disposition);
        }
        #endregion

        #region Ordering
        protected void AddOrderInfo<TModel, TResult>(Expression<Func<TModel, TResult>> fieldMapping, QueryOrderDirection direction) where TModel : class
        {
            var propertyName = GetPropertyName(fieldMapping);
            if (!HasProperty<TModel>(propertyName))
            {
                throw new ArgumentException("This property is not supported");
            }

            AddOrderInfo(propertyName, direction);
        }

        public void AddOrderInfo(string propertyName, QueryOrderDirection direction)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException($"{nameof(propertyName)} can not be null");
            }

            OrderInfos.Add(new QueryOrderInfo(propertyName, direction));
        }

        public void AddOrderInfoByDistance(QueryOrderDirection direction) => OrderInfosSpecials.Add(new QueryOrderInfo("distance", direction));
        #endregion
    }
}