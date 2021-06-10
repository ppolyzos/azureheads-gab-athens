using System.Threading.Tasks;

namespace Identity.Api.Data.Seed.Core
{
    public interface IDataSeeder
    {
        Task SeedAsync();
    }
}