using Identity.Api.Application.Models.Errors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Identity.Api.Controllers
{
    [ApiController]
    public class IdentityController : Controller
    {
        protected IActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null) { return BadRequest("Error parsing identity result"); }

            // No errors
            if (result.Succeeded) return null;

            if (result.Errors != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            if (ModelState.IsValid)
            {
                // No ModelState errors are available to send, so just return an empty BadRequest.
                return BadRequest();
            }

            return BadRequest(ModelState);
        }

        protected IActionResult GetErrorResult(SignInResult result)
        {
            if (result == null) { return BadRequest("Error parsing identity result"); }

            // No errors
            if (result.Succeeded) return null;

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("lockedout", "UserProfile account locked out.");
            }     
            
            if (result.IsNotAllowed)
            {
                ModelState.AddModelError("notallowed", "User account is not allowed.");
            }     

            if (ModelState.IsValid)
            {
                // No ModelState errors are available to send, so just return an empty BadRequest.
                return BadRequest(new ErrorResult(ErrorMessages.GetErrorMessage(ErrorCode.AuthFailed)));
            }

            return BadRequest(ModelState);
        }
    }
}
