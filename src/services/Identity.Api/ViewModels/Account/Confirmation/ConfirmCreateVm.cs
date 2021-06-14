using System.ComponentModel.DataAnnotations;

namespace Identity.Api.ViewModels.Account.Confirmation
{
    public class ConfirmCreateVm
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Code { get; set; }
    }
}
