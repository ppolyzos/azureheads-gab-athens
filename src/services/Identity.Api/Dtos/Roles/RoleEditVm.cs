using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Dtos.Roles
{
    public class RoleEditDto
    {
        [Required, MinLength(2)]
        public string OldName { get; set; }

        [Required, MinLength(2)]
        public string NewName { get; set; }
    }
}