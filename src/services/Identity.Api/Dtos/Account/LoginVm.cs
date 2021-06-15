using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Dtos.Account
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
