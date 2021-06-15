using AutoMapper;
using Identity.Api.Data.Models;

namespace Identity.Api.Dtos
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDto>();
        }
    }
}