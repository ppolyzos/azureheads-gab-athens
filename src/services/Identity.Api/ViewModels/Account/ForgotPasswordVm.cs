using System.ComponentModel.DataAnnotations;

namespace Identity.Api.ViewModels.Account
{
    public class ForgotPasswordVm
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
