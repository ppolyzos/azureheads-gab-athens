using System.Threading.Tasks;
using SendGrid;

namespace Identity.Api.Services
{
    public interface IEmailSender
    {
        Task<Response> SendEmailAsync(string email, string subject, string message);
        Task<Response> SendEmailAsync(string email, string templateId, object templateData);
    }
}
