using System.Collections;

namespace EventManagement.Core.Utilities.Response
{
    public interface ISearchResults
    {
        int Total { get; set; }
        IList Items { get; }
    }
}