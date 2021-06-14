using System;
using EventManagement.Core.Enumerations;

namespace EventManagement.Core.EF.Entity
{
    public class OrderInfo
    {
        #region Properties
        public Type FieldType { get; set; }
        public string FieldName { get; set; }
        public SortOrder Order { get; set; }
        
        #endregion

        #region Constructors
        public OrderInfo(Type fieldType, string fieldName, SortOrder order)
        {
            FieldType = fieldType;
            FieldName = fieldName;
            Order = order;
        } 
        #endregion

        public static OrderInfo[] Single(Type fieldType, string fieldName, SortOrder order)
        {
            return new[] { new OrderInfo(fieldType, fieldName, order) };
        }

        public override string ToString()
        {
            return $"{FieldName}:{Order}";
        }
    }
}