using AutoMapper;
using Identity.Api.Data.Models;

namespace Identity.Api.ViewModels
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserVm>();
        }
    }
}