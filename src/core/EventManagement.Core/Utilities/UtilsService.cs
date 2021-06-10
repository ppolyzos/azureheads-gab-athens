using System.Linq;

namespace EventManagement.Core.Utilities
{
    public class UtilsService
    {
        public string SplitText(string text, int take = 10, int skip = 0)
        {
            return string.Join(' ', text.Split(' ').Skip(skip).Take(take));
        }
    }
}