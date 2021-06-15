using AutoMapper;
using Identity.Api.Data.Models;

namespace Identity.Api.Dtos.Account.Registration
{
    public class RegistrationProfile : Profile
    {
        public RegistrationProfile()
        {
            CreateMap<ApplicationUser, RegisterDto>();
        }
    }
}
