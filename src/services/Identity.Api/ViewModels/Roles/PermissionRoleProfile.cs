using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.ViewModels.Roles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<IdentityRole, RoleVm>();
        }
    }
}