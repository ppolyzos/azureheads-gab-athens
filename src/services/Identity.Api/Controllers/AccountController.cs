using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Api.Application.Commands.Communication.Emails;
using Identity.Api.Application.Commands.Communication.Sms;
using Identity.Api.Application.Models;
using Identity.Api.Application.Models.Errors;
using Identity.Api.Data.Models;
using Identity.Api.Dtos.Account;
using Identity.Api.Dtos.Account.Confirmation;
using Identity.Api.Dtos.Account.Registration;
using Identity.Api.Services;
using MediatR;
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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            IMediator mediator,
            ILoggerFactory loggerFactory,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _mediator = mediator;
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger<AccountsController>();
        }

        [AllowAnonymous, HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterCreateDto model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser == null) return GetErrorResult(result);

                var signInResult = await _signInManager.CheckPasswordSignInAsync(existingUser, model.Password, false);
                if (!signInResult.Succeeded) return GetErrorResult(result);

                if (!existingUser.EmailConfirmed) await _mediator.Send(new EmailConfirmationCommand { User = user });
                if (!existingUser.PhoneNumberConfirmed)
                {
                    //await _mediator.Send(new SmsConfirmationCommand {User = user});
                    await _mediator.Send(new SmsVerifyLinkCommand { User = user });
                }
                return Ok(_mapper.Map<RegisterDto>(existingUser));
            }

            await _mediator.Send(new EmailConfirmationCommand { User = user });
            //await _mediator.Send(new SmsConfirmationCommand { User = user });
            await _mediator.Send(new SmsVerifyLinkCommand { User = user });

            _logger.LogInformation("UserProfile created a new account with password");

            return Ok(_mapper.Map<RegisterDto>(user));
        }

        [AllowAnonymous, HttpPost("confirm")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmCreateDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound(new ErrorResult("UserNotFound", "No user found with this id"));
            }

            var result = await _userManager.ConfirmEmailAsync(user, model.Code);
            return result.Succeeded ? Ok() : GetErrorResult(result);
        }

        [AllowAnonymous, HttpGet("confirm-phone")]
        public async Task<IActionResult> ConfirmPhoneGet([FromQuery] ConfirmCreateDto model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound(new ErrorResult("UserNotFound", "No user found with this id"));

            if (user.PhoneNumberConfirmed) return Ok("Your phone is already verified");

            var result = await _userManager.VerifyChangePhoneNumberTokenAsync(user, model.Code, user.PhoneNumber);
            if (!result)
            {
                return StatusCode((int)HttpStatusCode.Conflict,
                    new ErrorResult(ErrorMessages.GetErrorMessage(ErrorCode.UserPhoneNotConfirmed)));
            }

            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);
            return Ok("Your phone has been verified");
        }

        [AllowAnonymous, HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return NotFound();
            }

            await _mediator.Send(new ForgotPasswordCommand { User = user });

            return Ok(new ResponseResult("An email is sent to reset your password."));
        }

        [AllowAnonymous, HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return NotFound();

            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

            return result.Succeeded
                ? Ok(new ResponseResult(user))
                : GetErrorResult(result);
        }

        [AllowAnonymous, HttpPost("code")]
        public async Task<IActionResult> SendCode([FromBody] SendCodeDto model)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null) return NotFound();

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code)) return NotFound();

            var message = "Your security code is: " + code;
            var response = string.Empty;
            switch (model.SelectedProvider)
            {
                case "Email":
                    await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
                    response = "An email has been sent to confirm your account";
                    break;
                case "Phone":
                    await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
                    response = "An sms has been sent to confirm your account";
                    break;
            }

            return Ok(response);
        }

        [AllowAnonymous, HttpPost]
        public async Task<IActionResult> VerifyCode(VerifyCodeDto model)
        {
            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }


    }
}
