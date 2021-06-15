using System.Threading.Tasks;
using Identity.Api.Data.Models;
using Identity.Api.Dtos.Account;
using Identity.Api.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : IdentityController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthService _authService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager,
            IAuthService authService,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _authService = authService;
            _signInManager = signInManager;
        }

        [AllowAnonymous, HttpPost("token")]
        public async Task<IActionResult> GetToken([FromBody] LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return Unauthorized();

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe,
                    lockoutOnFailure: false);

            if (!result.Succeeded) return GetErrorResult(result);


            var jwt = await _authService.GenerateJwtAsync(user);
            return Ok(jwt);
        }
    }
}