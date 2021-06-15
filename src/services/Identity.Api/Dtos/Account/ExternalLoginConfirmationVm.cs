using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Dtos.Account
{
    public class ExternalLoginConfirmationDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
