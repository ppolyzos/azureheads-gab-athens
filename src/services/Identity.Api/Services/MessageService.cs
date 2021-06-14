using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Identity.Api.Application.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Identity.Api.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        private readonly ILogger _logger;
        private readonly SmsConfig _smsConfig;
        private readonly EmailConfig _emailConfig;
        private ISendGridClient _sendGridClient;

        public AuthMessageSender(IOptions<EmailConfig> emailConfig,
            IOptions<SmsConfig> smsConfig,
            ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AuthMessageSender>();
            _smsConfig = smsConfig.Value;
            _emailConfig = emailConfig.Value;
        }

        public async Task<Response> SendEmailAsync(string email, string subject, string message)
        {
            _sendGridClient = new SendGridClient(_emailConfig.ApiKey);
            var from = new EmailAddress(_emailConfig.FromEmail, _emailConfig.From);
            var to = new EmailAddress(email);
            var plainTextContent = Regex.Replace(message, "<[^>]*>", "");
            var emailMsg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, message);

            var response = await _sendGridClient.SendEmailAsync(emailMsg);
            _logger.LogInformation("An email has been send to {email} with status: {statusCode}", email,
                response.StatusCode);

            return response;
        }

        public async Task<Response> SendEmailAsync(string email, string templateId, object templateData)
        {
            _sendGridClient = new SendGridClient(_emailConfig.ApiKey);
            var from = new EmailAddress(_emailConfig.FromEmail, _emailConfig.From);
            var to = new EmailAddress(email);
            var template = MailHelper.CreateSingleTemplateEmail(from, to, templateId, templateData);
            var response = await _sendGridClient.SendEmailAsync(template);

            _logger.LogInformation("An email has been sent to {email} with status: {statusCode}", email,
                response.StatusCode);

            return response;
        }

        public Task<MessageResource> SendSmsAsync(string destination, string message)
        {
            destination = !destination.StartsWith("+") ? $"+{destination}" : destination;

            TwilioClient.Init(_smsConfig.AccountsId, _smsConfig.AuthToken);

            var twilioMsg = MessageResource.Create(
                body: message,
                from: new Twilio.Types.PhoneNumber(_smsConfig.FromPhone),
                to: new Twilio.Types.PhoneNumber(destination)
            );
            _logger.LogInformation("UserProfile created a new account with password");
            _logger.LogInformation("An email has been send to {destination} with status: {status}", destination,
                twilioMsg.Status);

            return Task.FromResult(twilioMsg);
        }
    }
}