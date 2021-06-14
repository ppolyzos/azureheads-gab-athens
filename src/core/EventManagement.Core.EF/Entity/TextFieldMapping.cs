using System;
using System.Linq.Expressions;
using EventManagement.Core.EF.Extensions;

namespace EventManagement.Core.EF.Entity
{
    public class TextFieldMapping<T>
    {
        public string FieldName { get; }
        public Expression<Func<T, string>> ExtProperty { get; }

        public TextFieldMapping(Enum fieldName, Expression<Func<T, string>> extProperty)
        {
            FieldName = fieldName.ToString();
            ExtProperty = extProperty;
        }

        public TextFieldMapping(Expression<Func<T, string>> extProperty)
        {
            FieldName = QueryExtensions.GetPropertyName(extProperty);
            ExtProperty = extProperty;
        }
    }
}