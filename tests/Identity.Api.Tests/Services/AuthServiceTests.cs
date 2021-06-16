using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Identity.Api.Application.Configuration;
using Identity.Api.Data.Models;
using Identity.Api.Services.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Identity.Api.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly AuthService _sut;
        private readonly IOptions<AuthenticationConfig> _authConfig;
        
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        public AuthServiceTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;

            _userManagerMock = MockUserManager(GetFakeUsers());
            _authConfig = Options.Create(new AuthenticationConfig
            {
                Secret = "QlzYwSEZYHvXS7jVVKjCsk0YJxA38nJ8==",
                Issuer = "https://app.com",
                Audience = "app_admin",
                ExpiresInMinutes = 1440
            });
            _sut = new AuthService(_userManagerMock.Object,
                _authConfig);
        }

        [Fact]
        public async Task FindCompanyCarAsync_ShouldReturnCar_WhenCarExistsInACompany()
        {
            // Arrange
            var applicationUser = new ApplicationUser { Email = "test@test.com", Id = Guid.NewGuid().ToString() };
            _userManagerMock.Setup(x => x.GetClaimsAsync(applicationUser))
                .ReturnsAsync(new List<Claim>());
            _userManagerMock.Setup(x => x.GetRolesAsync(applicationUser))
                .ReturnsAsync(new[] { "Admin" });

            // Act
            var response = await _sut.GenerateJwtAsync(applicationUser);

            // Assert
            response.Should().NotBeNull();
            var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(response.Token);

            decodedToken.Issuer.Should().Be(_authConfig.Value.Issuer);
            decodedToken.Audiences.FirstOrDefault().Should().Be(_authConfig.Value.Audience);

            var tokenExpires = DateTime.UtcNow.AddMinutes(_authConfig.Value.ExpiresInMinutes);
            decodedToken.ValidTo.Subtract(tokenExpires).TotalSeconds
                .Should()
                .BeNegative()
                .And.BeGreaterThan(-1);

            _testOutputHelper.WriteLine($"Token generated on {DateTime.UtcNow} and expires in {tokenExpires}");
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>(IList<TUser> ls) where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
            mgr.Object.UserValidators.Add(new UserValidator<TUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);
            mgr.Setup(x => x.CreateAsync(It.IsAny<TUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success)
                .Callback<TUser, string>((x, y) => ls.Add(x));
            mgr.Setup(x => x.UpdateAsync(It.IsAny<TUser>())).ReturnsAsync(IdentityResult.Success);

            return mgr;
        }

        private IList<ApplicationUser> GetFakeUsers()
        {
            return new List<ApplicationUser>
            {
                new ApplicationUser { Email = "test@test.com", Id = Guid.NewGuid().ToString() }
            };
        }
    }
}