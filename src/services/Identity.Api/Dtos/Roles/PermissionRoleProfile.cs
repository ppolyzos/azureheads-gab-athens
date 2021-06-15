using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Dtos.Roles
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<IdentityRole, RoleDto>();
        }
    }
}