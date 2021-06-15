using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using EventManagement.Core.EF.Entity;
using EventManagement.Core.EF.Queries;
using EventManagement.Core.Utilities.Response;

namespace EventManagement.Core.EF.Repositories
{
    public interface IRepository<TDto, TKey> where TDto:class
    {
        Task<TDto> FindAsync(object key);
        Task<TDto> FindOneAsync(IObjectQuery query);
        Task<IEnumerable<TDto>> FetchAsync(IObjectQuery query, bool readOnly = true);
        Task<SearchResults<TDto>> SearchAsync(IObjectQuery query);


        Task<int> CountAsync(IObjectQuery query);
        Task<bool> AnyAsync(IObjectQuery query);


        Task<TDto> SaveAsync(IModel instance, IList<string> keysToClear = null);
        Task<IEnumerable<TDto>> SaveAsync(IEnumerable<IModel> instances);
        Task StoreAsync(IModel instance);

        Task<bool> DeleteAsync(TDto instance);
        Task<bool> DeleteAsync(IEnumerable<TDto> instances);

        Task WithTransaction(Func<Task> transactionLoader);
        Task<T> WithTransaction<T>(Func<Task<T>> transactionLoader);

        DbCommand GetStoredProcedure(string storedProcedureName);
    }
}