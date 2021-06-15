using System.Threading.Tasks;
using AutoMapper;
using Identity.Api.Data.Repositories;
using Identity.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : IdentityController
    {
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;

        public TestController(ICurrentUser currentUser,
            IMapper mapper)
        {
            _currentUser = currentUser;
            _mapper = mapper;
        }

        [Authorize, HttpGet("current-user")]
        public async Task<IActionResult> CurrentUser()
        {
            var user = await _currentUser.GetAppUserAsync();
            return Ok(_mapper.Map<UserDto>(user));
        }
    }
}