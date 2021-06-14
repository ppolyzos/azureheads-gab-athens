using System.ComponentModel.DataAnnotations;

namespace Identity.Api.ViewModels.Account
{
    public class LoginVm
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
