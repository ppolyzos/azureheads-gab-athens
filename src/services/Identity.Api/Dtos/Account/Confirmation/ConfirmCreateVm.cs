using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Dtos.Account.Confirmation
{
    public class ConfirmCreateDto
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
