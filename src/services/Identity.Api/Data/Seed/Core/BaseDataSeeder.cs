using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Data.Seed.Core
{
    public abstract class BaseDataSeeder
    {
        protected readonly AuthDbContext DbContext;

        protected BaseDataSeeder(AuthDbContext dbContext)
        {
            DbContext = dbContext;
        }

        protected async Task SetIdentityInsert(string tableName, bool on)
        {
            var mode = on ? "ON" : "OFF";
            var sql = $"SET IDENTITY_INSERT {tableName} {mode};";

            await DbContext.Database.OpenConnectionAsync();
            await DbContext.Database.ExecuteSqlRawAsync(sql);
            await DbContext.Database.CloseConnectionAsync();
        }
    }
}