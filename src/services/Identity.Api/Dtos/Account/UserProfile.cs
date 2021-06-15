using AutoMapper;
using Identity.Api.Data.Models;

namespace Identity.Api.Dtos.Account
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDto>();
        }
    }
}
