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

namespace Identity.Api.Application.Commands.Communication.Sms
{
    public class SmsConfirmationCommand : IRequest<ResponseResult>
    {
        public ApplicationUser User { get; set; }
    }

    public class SmsConfirmationCommandHandler : IRequestHandler<SmsConfirmationCommand, ResponseResult>
    {
        private readonly ILogger _logger;
        private readonly ISmsSender _smsSender;
        private readonly AppSettings _appConfig;
        private readonly UserManager<ApplicationUser> _userManager;

        public SmsConfirmationCommandHandler(ISmsSender smsSender,
            IOptions<AppSettings> appConfig,
            ILoggerFactory loggerFactory,
            UserManager<ApplicationUser> userManager)
        {
            _smsSender = smsSender;
            _appConfig = appConfig.Value;
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<SmsConfirmationCommandHandler>();
        }

        public async Task<ResponseResult> Handle(SmsConfirmationCommand request, CancellationToken cancellationToken)
        {
            var user = request.User;
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

            var result = await _smsSender.SendSmsAsync(user.PhoneNumber,
                $"Your {_appConfig.ProjectName} code is: {code}");

            _logger.LogInformation("An sms is sent to verify user's phone number. code: {Code}, status: {@Status}", code, result.Status);

            return new ResponseResult(request);
        }
    }
}
