using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EventManagement.Core.EF.Extensions
{
    public static class SqlQueryExtensions
    {
        public static async Task<IList<T>> SqlQuery<T>(this DbContext db, string sql, params object[] parameters)
            where T : class
        {
            await using var db2 = new ContextForQueryType<T>(db.Database.GetDbConnection());
            return await db2.Set<T>()
                .FromSqlRaw(sql, parameters)
                .ToListAsync();
        }

        private class ContextForQueryType<T> : DbContext where T : class
        {
            private readonly DbConnection _connection;

            public ContextForQueryType(DbConnection connection)
            {
                this._connection = connection;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                // switch on the connection type name to enable support multiple providers
                // var name = con.GetType().Name;
                optionsBuilder.UseSqlServer(_connection, options => options.EnableRetryOnFailure());

                base.OnConfiguring(optionsBuilder);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<T>().HasNoKey();
                base.OnModelCreating(modelBuilder);
            }
        }
    }
}