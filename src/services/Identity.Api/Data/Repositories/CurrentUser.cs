using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Api.Data.Models;
using Identity.Api.Dtos.Account;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Data.Repositories
{
    public interface ICurrentUser
    {
        Task<UserDto> GetUserAsync();
        Task<ApplicationUser> GetAppUserAsync();
        string UserId { get; }
    }

    public class CurrentUser : ICurrentUser
    {
        private readonly HttpContext _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private string _userId;

        public string UserId => _userId ??= _httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public CurrentUser(IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<UserDto> GetUserAsync() => _mapper.Map<UserDto>(await GetAppUserAsync());

        public async Task<ApplicationUser> GetAppUserAsync() => await _userManager.GetUserAsync(_httpContext.User);
    }

    public interface IIdentityService
    {
        string UserId { get; }
    }

    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;

        private string _userId;

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public string UserId
        {
            get
            {
                if (_context.HttpContext == null)
                    return null;

                return _userId ??= _context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
        }
    }
}