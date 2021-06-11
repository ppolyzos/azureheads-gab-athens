using System.Linq;

namespace EventManagement.Core.Utilities
{
    public static class UtilsService
    {
        public static string SplitText(string text, int take = 10, int skip = 0)
        {
            return string.Join(' ', text.Split(' ').Skip(skip).Take(take));
        }
    }
}