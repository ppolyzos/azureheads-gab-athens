using System.Threading.Tasks;
using Identity.Api.Contracts.V1.Responses;
using Identity.Api.Data.Models;

namespace Identity.Api.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthSuccessResponse> GenerateJwtAsync(ApplicationUser user);
    }
}