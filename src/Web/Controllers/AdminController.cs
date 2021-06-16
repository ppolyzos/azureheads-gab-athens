using System;
using System.Threading.Tasks;
using EventManagement.Web.Application.Cache.Enumerations;
using EventManagement.Web.Application.Cache.Redis;
using EventManagement.Web.Integrations.Sessionize;
using EventManagement.Web.Services.Events;
using EventManagement.Web.Utilities;
using Identity.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace EventManagement.Web.Controllers
{
    [Authorize(Roles = Roles.Admin), Route("api/[controller]")]
    public class AdminController : Controller
    {
        private readonly IEventDetailsService _eventDetailsService;
        private readonly IEventService _eventService;
        private readonly ISessionizeService _sessionizeService;
        private readonly IEventSessionizeService _eventSessionizeService;
        private readonly IMemoryCache _memoryCache;

        public AdminController(IEventDetailsService eventDetailsService,
            IEventService eventService,
            ISessionizeService sessionizeService,
            IEventSessionizeService eventSessionizeService,
            IMemoryCache memoryCache)
        {
            _eventDetailsService = eventDetailsService;
            _eventService = eventService;
            _sessionizeService = sessionizeService;
            _eventSessionizeService = eventSessionizeService;
            _memoryCache = memoryCache;
        }

        [HttpGet("cached-event"), CacheApiResponse(CacheDuration.CacheLow)]
        public async Task<IActionResult> GetCachedEvent()
        {
            var eventContainer = Environment.GetEnvironmentVariable(Constants.EnvEventContainer) ?? "gab-events";
            var eventFile = Environment.GetEnvironmentVariable(Constants.EnvEventFile) ?? "ga-greece-2021.json";
            var eventDetails = await _eventService.FetchAsync(eventContainer, eventFile);
            return Ok(eventDetails);
        }
        

        [HttpPost("cache/refresh")]
        public async Task<IActionResult> CacheRefreshAsync([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key) ||
                !string.Equals(key, Environment.GetEnvironmentVariable(Constants.EnvAdminKey)))
                return BadRequest();

            _memoryCache.Remove(Constants.CacheEventDetailsKey);

            var eventFile = Environment.GetEnvironmentVariable(Constants.EnvEventFile) ?? "ga-greece-2021.json";
            var eventContainer = Environment.GetEnvironmentVariable(Constants.EnvEventContainer) ?? "gab-events";

            var content = await _eventDetailsService.FetchAsync(eventContainer, eventFile);
            
            _memoryCache.Remove(Constants.CacheSpeakersKey);
            _memoryCache.Remove(Constants.CacheSessionsKey);
            
            return Ok(content);
        }

        [HttpGet("current-event")]
        public async Task<IActionResult> GetCurrentEventAsync([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key) ||
                !string.Equals(key, Environment.GetEnvironmentVariable(Constants.EnvAdminKey)))
                return BadRequest();

            _memoryCache.Remove(Constants.CacheEventDetailsKey);

            var eventFile = Environment.GetEnvironmentVariable(Constants.EnvEventFile) ?? "ga-greece-2021.json";
            var eventContainer = Environment.GetEnvironmentVariable(Constants.EnvEventContainer) ?? "gab-events";

            var content = await _eventDetailsService.FetchAsync(eventContainer, eventFile);
            return Ok(content);
        }
        
        [HttpGet("sessionize/speakers")]
        public async Task<IActionResult> FetchSessionizeSpeakersAsync([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key) ||
                !string.Equals(key, Environment.GetEnvironmentVariable(Constants.EnvAdminKey)))
                return BadRequest();

            var speakers = await _sessionizeService.FetchSpeakersAsync();

            return Ok(speakers);
        }

        [HttpGet("sessionize/sessions")]
        public async Task<IActionResult> FetchSessionizeSessionsAsync([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key) ||
                !string.Equals(key, Environment.GetEnvironmentVariable(Constants.EnvAdminKey)))
                return BadRequest();

            var sessions = await _sessionizeService.FetchSessionsAsync();

            return Ok(sessions);
        }

        [HttpGet("sessions")]
        public async Task<IActionResult> FetchSessionsAsync([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key) ||
                !string.Equals(key, Environment.GetEnvironmentVariable(Constants.EnvAdminKey)))
                return BadRequest();

            var sessions = await _eventSessionizeService.FetchSessionsAsync();

            return Ok(sessions);
        }
    }
}