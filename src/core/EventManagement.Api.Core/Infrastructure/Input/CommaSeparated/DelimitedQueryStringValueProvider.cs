using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace EventManagement.Api.Core.Infrastructure.Input.CommaSeparated
{
    public class DelimitedQueryStringValueProvider : QueryStringValueProvider
    {
        private readonly CultureInfo _culture;
        private readonly char[] _delimiters;
        private readonly IQueryCollection _queryCollection;

        public char[] Delimiters => _delimiters;

        public DelimitedQueryStringValueProvider(
            BindingSource bindingSource,
            IQueryCollection values,
            CultureInfo culture,
            char[] delimiters)
            : base(bindingSource, values, culture)
        {
            _queryCollection = values;
            _culture = culture;
            _delimiters = delimiters;
        }

        public override ValueProviderResult GetValue(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var values = _queryCollection[key];
            if (values.Count == 0) return ValueProviderResult.None;

            if (!values.Any(x => _delimiters.Any(x.Contains)))
                return new ValueProviderResult(values, _culture);

            var stringValues = new StringValues(values
                .SelectMany(x => x.Split(_delimiters, StringSplitOptions.RemoveEmptyEntries))
                .ToArray());
            return new ValueProviderResult(stringValues, _culture);
        }
    }
}
