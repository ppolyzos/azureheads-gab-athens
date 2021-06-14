using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace Identity.Api.Services
{
    public interface ISmsSender
    {
        Task<MessageResource> SendSmsAsync(string destination, string message);
    }
}
