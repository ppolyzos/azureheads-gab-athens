using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Api.Data.Models;
using Identity.Api.Data.Seed.Core;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Data.Seed
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class UserDataSeeder : BaseDataSeeder, IDataSeeder
    {
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private IList<IdentityRole> _roles;

        public UserDataSeeder(AuthDbContext dbContext, IPasswordHasher<ApplicationUser> passwordHasher) :
            base(dbContext)
        {
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            _roles = await this.SeedRolesAsync();
            await this.SeedUsersAsync();
        }

        private async Task<IList<IdentityRole>> SeedRolesAsync()
        {
            if (DbContext.Roles.Any()) return new List<IdentityRole>();

            var roles = new[] { Roles.Admin, Roles.Company, Roles.Support }
                .Select(c => new IdentityRole(c) { NormalizedName = c.ToUpperInvariant() })
                .ToArray();

            await DbContext.Roles.AddRangeAsync(roles);
            await DbContext.SaveChangesAsync();

            return roles;
        }

        private async Task SeedUsersAsync()
        {
            if (DbContext.Users.Any()) return;

            await this.GenerateUser("admin", "P@ssword!!!", null, new[] { "Admin", "Company" },
                new Dictionary<string, string>
                {
                    { "RolePermissionsChange", "allow" }
                });

            await this.GenerateUser("company", "P@ssword!!!", null, new[] { "Company" }, null);

            await this.GenerateUser("demo", "P@ssword!!!",
                Guid.Parse("20fe3a6f-4016-45ce-9f0e-c850e66d4bac").ToString(), null, null);

            // Persist data
            await DbContext.SaveChangesAsync();
        }

        private async Task GenerateUser(string username, string password, string id, string[] roles,
            Dictionary<string, string> claims)
        {
            var user = this.CreateUser(username, password, id);
            await DbContext.Users.AddAsync(user);

            if (roles != null && roles.Any())
            {
                var roleIds = roles.Select(role => _roles.FirstOrDefault(
                        dbRole => dbRole.NormalizedName == role.ToUpperInvariant())?.Id)
                    .Where(r => r != null)
                    .ToArray();

                var userRoles = roleIds.Select(roleId => new IdentityUserRole<string>
                    { RoleId = roleId, UserId = user.Id });
                await DbContext.UserRoles.AddRangeAsync(userRoles);
            }

            if (claims == null || !claims.Any()) return;

            var userClaims = claims.Select(claim => new IdentityUserClaim<string>()
            {
                ClaimType = claim.Key,
                ClaimValue = claim.Value,
                UserId = user.Id
            });
            await DbContext.UserClaims.AddRangeAsync(userClaims);
        }

        private ApplicationUser CreateUser(string username, string password, string id = null)
        {
            var user = new ApplicationUser
            {
                Id = id ?? Guid.NewGuid().ToString(),
                UserName = username,
                Email = $"{username}@ppolyzos.com",
                PhoneNumber = "1234567890",
                NormalizedEmail = $"{username}@ppolyzos.com".ToUpperInvariant(),
                NormalizedUserName = username.ToUpperInvariant(),
                SecurityStamp = Guid.NewGuid().ToString("D")
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            return user;
        }
    }
}