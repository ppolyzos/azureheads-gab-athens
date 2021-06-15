using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    public class ForgotPasswordCommand : IRequest<ResponseResult>
    {
        public ApplicationUser User { get; set; }
    }

    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, ResponseResult>
    {
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailConfig _emailConfig;

        public ForgotPasswordCommandHandler(IEmailSender emailSender,
            ILoggerFactory loggerFactory,
            IOptions<EmailConfig> emailConfig,
            UserManager<ApplicationUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _emailConfig = emailConfig.Value;
            _logger = loggerFactory.CreateLogger<ForgotPasswordCommandHandler>();
        }

        public async Task<ResponseResult> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(request.User);

            var t = await _emailSender.SendEmailAsync(request.User.Email,
                _emailConfig.Templates[EmailConfig.Template.EmailConfirm], new Dictionary<string, object>
                {
                    {"confirmUrl", $"{_emailConfig.ConfirmUrl}?code={code}"} 
                });

            _logger.LogInformation("User forgot his password. Email sent: {statusCode}", t.StatusCode);

            return new ResponseResult(request);
        }
    }
}
