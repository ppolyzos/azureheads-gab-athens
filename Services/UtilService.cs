using System.Linq;

namespace gab_athens.Services
{
    public class UtilService
    {
        public static string SplitText(string text, int take = 10, int skip = 0)
        {
            return string.Join(' ', text.Split(' ').Skip(skip).Take(take));
        }
    }
}