using System.Threading.Tasks;
using EventManagement.Core.EF.Repositories;
using Identity.Api.Data.Models;
using Identity.Api.Data.Queries;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Data.Repositories
{
    public interface IUsersRepository : IRepository<ApplicationUser, string>
    {
        Task<ApplicationUser> FindByUserIdAsync(string userId);
    }

    public class UsersRepository : CommonRepository<ApplicationUser, UsersQuery>, IUsersRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersRepository(AuthDbContext db,
            UserManager<ApplicationUser> userManager)
            : base(db, db.Users)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> FindByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            return await _userManager.FindByIdAsync(userId);
        }
    }
}
