using System;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using EventManagement.Core.Enumerations;

namespace EventManagement.Core.EF.Queries
{
    public class TextFieldQuery : Query
    {
        [JsonPropertyName("f")]
        public string FieldName { get; set; }
        public int LanguageId { get; set; }

        [JsonPropertyName("op")]
        public TextFieldValueOperator Operator { get; set; }

        [JsonPropertyName("v")]
        public string Value { get; set; }


        public Expression<Func<T, bool>> BuildFilter<T>(Expression<Func<T, string>> extProperty, string value)
        {
            Expression methodCall;

            switch (Operator)
            {
                case TextFieldValueOperator.StartsWith:
                    methodCall = Expression.Call(extProperty.Body, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), Expression.Constant(value));
                    break;
                case TextFieldValueOperator.EndsWith:
                    methodCall = Expression.Call(extProperty.Body, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), Expression.Constant(value));
                    break;
                case TextFieldValueOperator.Equals:
                    methodCall = Expression.Call(extProperty.Body, typeof(string).GetMethod("Equals", new[] { typeof(string) }), Expression.Constant(value));
                    break;
                case TextFieldValueOperator.NotEquals:
                    methodCall = Expression.Not(Expression.Call(extProperty.Body, typeof(string).GetMethod("Equals", new[] { typeof(string) }), Expression.Constant(value)));
                    break;
                case TextFieldValueOperator.NotContains:
                    methodCall = Expression.Not(Expression.Call(extProperty.Body, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(value)));
                    break;
                case TextFieldValueOperator.Contains:
                    methodCall = Expression.Call(extProperty.Body, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(value));
                    break;
                default:
                    methodCall = Expression.Call(extProperty.Body, typeof(string).GetMethod("Contains", new[] { typeof(string) }), Expression.Constant(value));
                    break;
            }

            return Expression.Lambda<Func<T, bool>>(methodCall, extProperty.Parameters[0]);
        }
    }
}