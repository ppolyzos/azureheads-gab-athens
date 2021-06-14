using System.ComponentModel.DataAnnotations;

namespace Identity.Api.ViewModels.Account
{
    public class ExternalLoginConfirmationVm
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
