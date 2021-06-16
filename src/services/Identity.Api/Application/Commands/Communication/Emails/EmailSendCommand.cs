using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Identity.Api.Application.Configuration;
using Identity.Api.Application.Models;
using Identity.Api.Data.Models;
using Identity.Api.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Api.Application.Commands.Communication.Emails
{
    public class EmailConfirmationCommand : IRequest<ResponseResult>
    {
        public ApplicationUser User { get; set; }
    }

    public class EmailConfirmationCommandHandler : IRequestHandler<EmailConfirmationCommand, ResponseResult>
    {
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailConfig _emailConfig;

        public EmailConfirmationCommandHandler(IEmailSender emailSender,
            ILoggerFactory loggerFactory,
            IOptions<EmailConfig> emailConfig,
            UserManager<ApplicationUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _emailConfig = emailConfig.Value;
            _logger = loggerFactory.CreateLogger<EmailConfirmationCommandHandler>();
        }

        public async Task<ResponseResult> Handle(EmailConfirmationCommand request, CancellationToken cancellationToken)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(request.User);
            var encodedToken = HttpUtility.UrlEncode(code);

            var t = await _emailSender.SendEmailAsync(request.User.Email,
                _emailConfig.Templates[EmailConfig.Template.EmailConfirm], new Dictionary<string, object>
                {
                    { "confirmUrl", $"{_emailConfig.ConfirmUrl}?userId={request.User.Id}&code={encodedToken}" }
                });

            _logger.LogInformation("An email is sent to verify user's account. Status: {StatusCode}", t.StatusCode);

            return new ResponseResult(request);
        }
    }
}