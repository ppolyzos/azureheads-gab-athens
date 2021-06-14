using AutoMapper;
using Identity.Api.Data.Models;

namespace Identity.Api.ViewModels.Account.Registration
{
    public class RegistrationProfile : Profile
    {
        public RegistrationProfile()
        {
            CreateMap<ApplicationUser, RegisterVm>();
        }
    }
}
