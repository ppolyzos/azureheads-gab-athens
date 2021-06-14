using AutoMapper;
using Identity.Api.Data.Models;

namespace Identity.Api.ViewModels.Account
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserVm>();
        }
    }
}
