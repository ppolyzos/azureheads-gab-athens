using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EventManagement.Core.Utilities.Response
{
    public class SearchResults<T> : ISearchResults
    {
        public int Total { get; set; }

        public IEnumerable<T> Results { get; }

        public IList Items => Results?.ToList();

        public SearchResults(int total, IEnumerable<T> results)
        {
            Total = total;
            Results = results;
        }
    }
}
