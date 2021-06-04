using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EventManagement.Web.Infrastructure.Input.CommaSeparated
{
    public class DelimitedQueryStringValueProviderFactory : IValueProviderFactory
    {
        private static readonly char[] DefaultDelimiters = new char[] { ',' };
        private readonly char[] _delimiters;

        public DelimitedQueryStringValueProviderFactory()
            : this(DefaultDelimiters)
        {
        }

        public DelimitedQueryStringValueProviderFactory(params char[] delimiters)
        {
            if (delimiters == null || delimiters.Length == 0)
            {
                _delimiters = DefaultDelimiters;
            }
            else
            {
                _delimiters = delimiters;
            }
        }

        /// <inheritdoc />
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var valueProvider = new DelimitedQueryStringValueProvider(
                BindingSource.Query,
                context.ActionContext.HttpContext.Request.Query,
                CultureInfo.InvariantCulture,
                _delimiters);

            context.ValueProviders.Add(valueProvider);

            return Task.CompletedTask;
        }
    }
}