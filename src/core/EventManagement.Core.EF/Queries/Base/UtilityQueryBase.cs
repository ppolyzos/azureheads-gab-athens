using System;
using System.Linq.Expressions;
using EventManagement.Core.Enumerations;

namespace EventManagement.Core.EF.Queries.Base
{
    public abstract class UtilityQueryBase<TModel> : CommonQueryBase where TModel : class
    {
        public void AddOrderInfo<TResult>(Expression<Func<TModel, TResult>> fieldMapping, QueryOrderDirection direction) => AddOrderInfo<TModel, TResult>(fieldMapping, direction);

        public void AddTextFieldQuery<TResult>(Expression<Func<TModel, TResult>> fieldMapping,
            TextFieldValueOperator op,
            string value,
            QueryDisposition disposition = QueryDisposition.Optional) => AddTextFieldExpressionQuery(fieldMapping, op, value, disposition);
    }
}