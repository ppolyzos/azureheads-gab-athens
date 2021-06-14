using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
    public class SmsVerifyLinkCommand : IRequest<ResponseResult>
    {
        public ApplicationUser User { get; set; }
    }

    public class SmsVerifyLinkCommandHandler : IRequestHandler<SmsVerifyLinkCommand, ResponseResult>
    {
        private readonly ILogger _logger;
        private readonly ISmsSender _smsSender;
        private readonly AppSettings _appConfig;
        private readonly UserManager<ApplicationUser> _userManager;

        public SmsVerifyLinkCommandHandler(ISmsSender smsSender,
            IOptions<AppSettings> appConfig,
            ILoggerFactory loggerFactory,
            UserManager<ApplicationUser> userManager)
        {
            _smsSender = smsSender;
            _appConfig = appConfig.Value;
            _userManager = userManager;
            _logger = loggerFactory.CreateLogger<SmsConfirmationCommandHandler>();
        }

        public async Task<ResponseResult> Handle(SmsVerifyLinkCommand request, CancellationToken cancellationToken)
        {
            var user = request.User;
            var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);

            var apiUrl = $"{_appConfig.ProjectUrl}/api/accounts/confirm-phone?userId={user.Id}&code={code}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://tinyurl.com");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync($"api-create.php?url={apiUrl}", cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Error creating tinyUrl to verify user's phone number using link. code: {code}, status: {statusCode}", code, response.StatusCode);
                }
                else
                {
                    var link = await response.Content.ReadAsStringAsync(cancellationToken);
                    var result = await _smsSender.SendSmsAsync(user.PhoneNumber,
                        $"Verify your {_appConfig.ProjectName} account by clicking this link: {link}");
                    _logger.LogInformation("An sms is sent to verify user's phone number using link. code: {code}, status: ${status}", code,
                        result.Status);
                }
            }

            return new ResponseResult(request);
        }
    }
}
