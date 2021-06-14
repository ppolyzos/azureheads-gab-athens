using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EventManagement.Core.EF.Entity;
using EventManagement.Core.EF.Queries;
using EventManagement.Core.EF.Queries.Base;
using EventManagement.Core.Enumerations;
using EventManagement.Core.Utilities.Expressions;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Core.EF.Extensions
{
    /// <summary>
    /// Static class to help build complex queries
    /// </summary>
    public static class QueryExtensions
    {
        public static IQueryable<T> With<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> expression)
        {
            query = condition ? query.Where(expression) : query;
            return query;
        }

        public static IQueryable<T> With<T>(this IQueryable<T> query, bool condition, IQueryable<T> queryExecution)
        {
            // TODO: Find why this is not working when chaining events
            query = condition ? queryExecution : query;
            return query;
        }

        public static IQueryable<T> WithContains<T, TCollection>(this IQueryable<T> query,
            ICollection<TCollection> collection, Expression<Func<T, TCollection>> getFunc)
        {
            return collection != null && collection.Any() ? query.Where(ContainsExpression(collection, getFunc)) : query;
        }

        public static IQueryable<T> WithExcludes<T, TCollection>(this IQueryable<T> query,
            ICollection<TCollection> collection, Expression<Func<T, TCollection>> getFunc)
        {
            return collection != null && collection.Any() ? query.Where(ContainsExpression(collection, getFunc, not: true)) : query;
        }

        private static Expression<Func<T, bool>> ContainsExpression<T, TCollection>(ICollection<TCollection> collectionValue, Expression<Func<T, TCollection>> getFunc, bool not = false)
        {
            var paramExp = Expression.Parameter(typeof(T), "o");
            var collectionExp = Expression.Constant(collectionValue.AsEnumerable());
            var containsMethod = typeof(ICollection<TCollection>).GetMethod("Contains", new[] { typeof(TCollection) });
            var valueExp = Expression.Invoke(getFunc, paramExp);
            var containsExp = (Expression) Expression.Call(collectionExp, containsMethod, valueExp);

            if (not)
            {
                containsExp = Expression.Not(containsExp);
            }

            return Expression.Lambda<Func<T, bool>>(containsExp, paramExp);
        }

        public static IQueryable<TOuter> JoinIf<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> query, bool condition,
            IEnumerable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            query = condition
                ? query.Join(inner, outerKeySelector, innerKeySelector, resultSelector) as IQueryable<TOuter>
                : query;

            return query;
        }

        public static IQueryable<T> BuildOrder<T>(this IQueryable<T> query, QueryOrderInfo queryOrderInfo)
        {
            if (queryOrderInfo == null || query == null) throw new ArgumentNullException(nameof(queryOrderInfo));

            var queryElementTypeParam = Expression.Parameter(typeof(T));

            var memberAccess = Expression.PropertyOrField(queryElementTypeParam, queryOrderInfo.OrderByProperty);
            var keySelector = Expression.Lambda(memberAccess, queryElementTypeParam);

            var orderBy = Expression.Call(
                typeof(Queryable),
                queryOrderInfo.OrderDirection == QueryOrderDirection.Ascending ? "OrderBy" : "OrderByDescending",
                new[] { typeof(T), memberAccess.Type },
                query.Expression,
                Expression.Quote(keySelector));

            return query.Provider.CreateQuery<T>(orderBy);
        }

        public static Expression<Func<TResult, bool>> BuildFilter<TResult>(this IEnumerable<TextFieldQuery> translatableFieldQueries, TextFieldMapping<TResult>[] fieldMappings)
            where TResult : class
        {
            if (translatableFieldQueries == null) throw new ArgumentNullException(nameof(translatableFieldQueries));

            Expression<Func<TResult, bool>> filter = null;

            foreach (var translatableFieldQuery in translatableFieldQueries)
            {
                var fieldMapping = fieldMappings.FirstOrDefault(m => m.FieldName == translatableFieldQuery.FieldName);
                if (fieldMapping == null) throw new ArgumentNullException($"Cannot locate mapping for field {translatableFieldQuery.FieldName}");

                var singleFilter = translatableFieldQuery.BuildFilter(fieldMapping.ExtProperty, translatableFieldQuery.Value);

                if (filter == null)
                {
                    filter = singleFilter;
                }
                else
                {
                    switch (translatableFieldQuery.Disposition)
                    {
                        case QueryDisposition.Required:
                            filter = filter.And(singleFilter);
                            break;

                        case QueryDisposition.Optional:
                            filter = filter.Or(singleFilter);
                            break;

                        case QueryDisposition.Forbidden:
                            throw new ArgumentException("Forbidden translatable queries are not supported.");
                    }
                }
            }

            return filter;
        }

        public static IQueryable<T> BuildQueries<T>(this IEnumerable<CommonQueryBase> queries, DbContext context)
            where T : class
        {
            IQueryable<T> query = null;

            foreach (var subQuery in queries)
            {
                var linqQuery = subQuery.BuildQuery<T>(context);

                if (query == null)
                {
                    query = linqQuery; // Initial query upon which all subqueries are build on
                }
                else
                {
                    switch (subQuery.Disposition)
                    {
                        case QueryDisposition.Required:
                            query = query.Concat(linqQuery);
                            break;

                        case QueryDisposition.Optional:
                            query = query.Union(linqQuery);
                            break;

                        case QueryDisposition.Forbidden:
                            query = query.Except(linqQuery);
                            break;
                    }
                }
            }

            return query;
        }

        #region FullText Search
        /// <summary>
        /// Returns a dictionary with key the column name and value the sql string that must be passed as parameter
        /// to the Sql Server's CONTAINS() method, in order to perform the full-text search on this column
        /// </summary>
        public static Dictionary<string, string> GetFullTextSearchConditions(this IEnumerable<TextFieldQuery> textFieldQueries)
        {
            if (textFieldQueries == null) throw new ArgumentNullException(nameof(textFieldQueries));

            var fullTextQueries = new Dictionary<string, string>();

            foreach (var tfq in textFieldQueries)
            {
                if (fullTextQueries.ContainsKey(tfq.FieldName))
                {
                    fullTextQueries[tfq.FieldName] = fullTextQueries[tfq.FieldName] += GetFullTextString(tfq);
                }
                else
                {
                    fullTextQueries.Add(tfq.FieldName, GetFullTextString(tfq));
                }
            }

            for (var i = 0; i < fullTextQueries.Count; i++)
            {
                var kv = fullTextQueries.ElementAt(i);

                var value = kv.Value.Trim();

                if (value.EndsWith("AND"))
                {
                    value = value.Substring(0, value.Length - 3);
                }
                else if (value.EndsWith("OR"))
                {
                    value = value.Substring(0, value.Length - 2);
                }
                else if (value.EndsWith("AND NOT"))
                {
                    value = value.Substring(0, value.Length - 7);
                }

                fullTextQueries[kv.Key] = value;
            }

            return fullTextQueries;
        }

        private static string GetFullTextString(this TextFieldQuery fq)
        {
            if (fq == null) throw new ArgumentNullException(nameof(fq));

            string fullTextString;

            switch (fq.Operator)
            {
                case TextFieldValueOperator.Contains:
                case TextFieldValueOperator.Equals:
                    fullTextString = $"\"{fq.Value}\"";
                    break;

                case TextFieldValueOperator.StartsWith:
                    fullTextString = $"\"{fq.Value}*\"";
                    break;

                default:
                    throw new ApplicationException("Only Contains, Equals and EndsWith operators are allowed");
            }

            switch (fq.Disposition)
            {
                case QueryDisposition.Required:
                    fullTextString += " AND ";
                    break;

                case QueryDisposition.Optional:
                    fullTextString += " OR ";
                    break;

                case QueryDisposition.Forbidden:
                    fullTextString += " AND NOT ";
                    break;
            }

            return fullTextString;
        }
        #endregion

        #region Utility
        public static bool HasProperty<T>(string propertyName) => !string.IsNullOrEmpty(propertyName) && typeof(T).GetProperty(propertyName) != null;

        public static string GetPropertyName<T, TResult>(Expression<Func<T, TResult>> fieldMapping)
        {
            return GetProperty(fieldMapping)?.Name;
        }

        public static PropertyInfo GetProperty<T, TResult>(Expression<Func<T, TResult>> fieldMapping)
        {
            var member = fieldMapping.Body as MemberExpression;
            return member?.Member as PropertyInfo;
        }
        #endregion
    }
}
