using System.ComponentModel.DataAnnotations;

namespace Identity.Api.ViewModels.Roles
{
    public class RoleEditVm
    {
        [Required, MinLength(2)]
        public string OldName { get; set; }

        [Required, MinLength(2)]
        public string NewName { get; set; }
    }
}