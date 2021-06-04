using System;

namespace EventManagement.Core.Utilities
{
    public class EnumHelper
    {
        public static T GetEnumProperty<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}