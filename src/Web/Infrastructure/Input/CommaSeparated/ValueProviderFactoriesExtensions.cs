using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EventManagement.Web.Infrastructure.Input.CommaSeparated
{
    public static class ValueProviderFactoriesExtensions
    {
        public static void AddDelimitedValueProviderFactory(
            this IList<IValueProviderFactory> valueProviderFactories,
            params char[] delimiters)
        {
            var queryStringValueProviderFactory = valueProviderFactories
                .OfType<QueryStringValueProviderFactory>()
                .FirstOrDefault();

            if (queryStringValueProviderFactory == null)
            {
                valueProviderFactories.Insert(
                    0, new DelimitedQueryStringValueProviderFactory(delimiters));
            }
            else
            {
                valueProviderFactories.Insert(
                    valueProviderFactories.IndexOf(queryStringValueProviderFactory),
                    new DelimitedQueryStringValueProviderFactory(delimiters));

                valueProviderFactories.Remove(queryStringValueProviderFactory);
            }
        }
    }
}