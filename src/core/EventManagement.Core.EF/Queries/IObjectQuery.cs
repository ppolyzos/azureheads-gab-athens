using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Core.EF.Queries
{
    public interface IObjectQuery
    {
        IQueryable<TModel> BuildQuery<TModel>(DbContext dbContext)
            where TModel : class;

        string CacheKey { get; set; }

        string GroupKey { get; set; }

        int? NumberOfItemsToSkip { get; set; }
        int? NumberOfItemsToReturn { get; set; }        

        object Key { get; set; }
    }
}