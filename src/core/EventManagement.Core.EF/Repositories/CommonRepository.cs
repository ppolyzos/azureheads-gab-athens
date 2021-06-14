using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using EventManagement.Core.EF.Entity;
using EventManagement.Core.EF.Queries;
using EventManagement.Core.EF.Queries.Sql;
using EventManagement.Core.Utilities.Response;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Core.EF.Repositories
{
    public abstract class CommonRepository<T, TQuery>
        where T : class
        where TQuery : IObjectQuery
    {
        protected readonly DbContext Db;
        protected readonly DbSet<T> DbSet;

        protected CommonRepository(DbContext db, DbSet<T> dbSet)
        {
            Db = db;
            DbSet = dbSet;
        }

        #region Find
        public async Task<T> FindAsync(object key)
        {
            var query = Activator.CreateInstance<TQuery>();
            query.Key = key;
            return await FindOneAsync(query);
        }

        public async Task<T> FindOneAsync(IObjectQuery query)
        {
            return await query.BuildQuery<T>(Db)
                //.AsNoTracking()
                .FirstOrDefaultAsync();
        }
        #endregion

        #region Fetch
        public async Task<IEnumerable<T>> FetchAsync(IObjectQuery query, bool readOnly = true)
        {
            var modelQuery = query.BuildQuery<T>(Db);
            var q = readOnly ? modelQuery.AsNoTracking() : modelQuery;
            return await q.ToArrayAsync();
        }
        #endregion

        #region Count / Search / Any

        public async Task<bool> AnyAsync(IObjectQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query), "query parameter should not be null");

            return await CountAsync(query) > 0;
        }

        public async Task<int> CountAsync(IObjectQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query), "query parameter should not be null");

            var countQuery = BuildCountQuery(query);

            return await countQuery
                .AsNoTracking()
                .CountAsync();
        }

        public async Task<SearchResults<T>> SearchAsync(IObjectQuery query)
        {
            if (query == null) throw new ArgumentNullException(nameof(query), "query parameter should not be null");

            // TODO: use single db call for count and results

            return new SearchResults<T>(await CountAsync(query),
                await FetchAsync(query));
        }
        #endregion

        #region Delete
        public async Task<bool> DeleteAsync(T instance)
        {
            if (instance == null) throw new ArgumentNullException($"{nameof(instance)} can not be null");

            Db.Entry(instance).State = EntityState.Deleted;
            return await Db.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(IEnumerable<T> instances)
        {
            var items = instances as T[] ?? instances.ToArray();
            if (!items.Any()) { return true; }

            foreach (var instance in items)
            {
                Db.Entry(instance).State = EntityState.Deleted;
            }
            return await Db.SaveChangesAsync() > 0;
        }
        #endregion

        #region Save / Update

        public async Task StoreAsync(IModel instance)
        {
            if (instance == null) throw new ArgumentNullException($"{nameof(instance)} can not be null");
            if (instance.Id <= 0)
            {
                DbSet.Add((T)instance);
            }
            else
            {
                Db.Entry(instance).State = EntityState.Modified;
            }
            await Db.SaveChangesAsync();
        }

        public async Task<T> SaveAsync(IModel instance, IList<string> keysToClear = null)
        {
            if (instance.Id <= 0)
            {
                DbSet.Add((T)instance);
            }
            else
            {
                Db.Entry(instance).State = EntityState.Modified;
            }
            return await Db.SaveChangesAsync() > 0 ? (T)instance : null;
        }

        public async Task<IEnumerable<T>> SaveAsync(IEnumerable<IModel> instances)
        {
            var enumerable = instances as IModel[] ?? instances.ToArray();

            if (!enumerable.Any())
                return (IEnumerable<T>)enumerable;

            foreach (var instance in enumerable)
            {
                if (instance.Id <= 0)
                {
                    DbSet.Add((T)instance);
                }
                else
                {
                    Db.Entry(instance).State = EntityState.Modified;
                }
            }
            return await Db.SaveChangesAsync() > 0 ? (IEnumerable<T>)enumerable : null;
        }
        #endregion

        #region Transactions
        public async Task WithTransaction(Func<Task> transactionLoader)
        {
            var strategy = Db.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                using (var dbContextTransaction = Db.Database.BeginTransaction())
                {
                    try
                    {
                        await transactionLoader.Invoke();

                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            });
        }

        public async Task<TResult> WithTransaction<TResult>(Func<Task<TResult>> transactionLoader)
        {
            var strategy = Db.Database.CreateExecutionStrategy();
            object item = null;
            await strategy.ExecuteAsync(async () =>
            {
                using (var dbContextTransaction = Db.Database.BeginTransaction())
                {
                    try
                    {
                        item = await transactionLoader.Invoke();

                        dbContextTransaction.Commit();
                    }
                    catch (Exception)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            });
            return (TResult)item;
        }
        #endregion

        #region Queries

        #endregion
        public DbCommand GetStoredProcedure(string storedProcedureName)
        {
            return Db.LoadStoredProcedure(storedProcedureName);
        }
        #region Helper Methods

        private IQueryable<T> BuildCountQuery(IObjectQuery query)
        {
            var numberOfItemsToReturn = query.NumberOfItemsToReturn;
            var numberOfItemToSkip = query.NumberOfItemsToSkip;

            query.NumberOfItemsToReturn = query.NumberOfItemsToSkip = null;

            var modelQuery = query.BuildQuery<T>(Db);

            query.NumberOfItemsToReturn = numberOfItemsToReturn;
            query.NumberOfItemsToSkip = numberOfItemToSkip;

            return modelQuery;
        }

        #endregion
    }
}