using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Dtos.Account
{
    public class ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }
}
