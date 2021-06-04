using System;

namespace EventManagement.Core.Utilities
{
    public static class Extensions
    {
        public static bool IsGuid(this string input)
        {
            return !string.IsNullOrEmpty(input)
                && Guid.TryParse(input, out var _);
        }
    }
}
