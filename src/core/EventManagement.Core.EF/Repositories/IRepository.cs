using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using EventManagement.Core.EF.Entity;
using EventManagement.Core.EF.Queries;
using EventManagement.Core.Utilities.Response;

namespace EventManagement.Core.EF.Repositories
{
    public interface IRepository<TVm, TKey> where TVm:class
    {
        Task<TVm> FindAsync(object key);
        Task<TVm> FindOneAsync(IObjectQuery query);
        Task<IEnumerable<TVm>> FetchAsync(IObjectQuery query, bool readOnly = true);
        Task<SearchResults<TVm>> SearchAsync(IObjectQuery query);


        Task<int> CountAsync(IObjectQuery query);
        Task<bool> AnyAsync(IObjectQuery query);


        Task<TVm> SaveAsync(IModel instance, IList<string> keysToClear = null);
        Task<IEnumerable<TVm>> SaveAsync(IEnumerable<IModel> instances);
        Task StoreAsync(IModel instance);

        Task<bool> DeleteAsync(TVm instance);
        Task<bool> DeleteAsync(IEnumerable<TVm> instances);

        Task WithTransaction(Func<Task> transactionLoader);
        Task<T> WithTransaction<T>(Func<Task<T>> transactionLoader);

        DbCommand GetStoredProcedure(string storedProcedureName);
    }
}