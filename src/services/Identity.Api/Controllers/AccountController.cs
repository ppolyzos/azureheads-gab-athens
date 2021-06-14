using System.Threading.Tasks;
using Identity.Api.Application.Models.Errors;
using Identity.Api.Data.Models;
using Identity.Api.ViewModels.Account.Confirmation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Identity.Api.Controllers
{
    [Authorize, Route("api/[controller]")]
    public class AccountsController : IdentityController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger _logger;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<AccountsController>();
        }

        [AllowAnonymous, HttpPost("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmCreateVm model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound(new ErrorResult("UserNotFound", "No user found with this id"));
            }

            _logger.LogInformation("user with id {userId} has been found", user.Id);

            var result = await _userManager.ConfirmEmailAsync(user, model.Code);
            return result.Succeeded ? Ok() : GetErrorResult(result);
        }
    }
}