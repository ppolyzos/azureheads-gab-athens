using System;
using System.Threading.Tasks;
using gab_athens.Services;
using gab_athens.Services.Storage;
using gab_athens.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace gab_athens.Controllers
{
    public class AdminController : Controller
    {
        private readonly IEventDataStorageService _eventDataStorageService;
        private readonly ISessionizeService _sessionizeService;
        private readonly IEventSessionizeService _eventSessionizeService;
        private readonly IMemoryCache _memoryCache;

        public AdminController(IEventDataStorageService eventDataStorageService,
            ISessionizeService sessionizeService,
            IEventSessionizeService eventSessionizeService,
            IMemoryCache memoryCache)
        {
            _eventDataStorageService = eventDataStorageService;
            _sessionizeService = sessionizeService;
            _eventSessionizeService = eventSessionizeService;
            _memoryCache = memoryCache;
        }

        [HttpPost("api/cache/refresh")]
        public async Task<IActionResult> CacheRefreshAsync([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key) ||
                !string.Equals(key, Environment.GetEnvironmentVariable(Constants.EnvAdminKey)))
                return BadRequest();

            _memoryCache.Remove(Constants.CacheEventsKey);

            var eventFile = Environment.GetEnvironmentVariable(Constants.EnvEventFile) ?? "ga-greece-2021.json";
            var eventContainer = Environment.GetEnvironmentVariable(Constants.EnvEventContainer) ?? "gab-events";

            var content = await _eventDataStorageService.FetchEventDetailsAsync(eventContainer, eventFile);
            
            _memoryCache.Remove(Constants.CacheSpeakersKey);
            _memoryCache.Remove(Constants.CacheSessionsKey);
            
            return Ok(content);
        }

        [HttpGet("api/current-event")]
        public async Task<IActionResult> GetCurrentEventAsync([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key) ||
                !string.Equals(key, Environment.GetEnvironmentVariable(Constants.EnvAdminKey)))
                return BadRequest();

            _memoryCache.Remove(Constants.CacheEventsKey);

            var eventFile = Environment.GetEnvironmentVariable(Constants.EnvEventFile) ?? "ga-greece-2021.json";
            var eventContainer = Environment.GetEnvironmentVariable(Constants.EnvEventContainer) ?? "gab-events";

            var content = await _eventDataStorageService.FetchEventDetailsAsync(eventContainer, eventFile);
            return Ok(content);
        }
        
        [HttpGet("api/admin/sessionize/speakers")]
        public async Task<IActionResult> FetchSessionizeSpeakersAsync([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key) ||
                !string.Equals(key, Environment.GetEnvironmentVariable(Constants.EnvAdminKey)))
                return BadRequest();

            var speakers = await _sessionizeService.FetchSpeakersAsync();

            return Ok(speakers);
        }

        [HttpGet("api/admin/sessionize/sessions")]
        public async Task<IActionResult> FetchSessionizeSessionsAsync([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key) ||
                !string.Equals(key, Environment.GetEnvironmentVariable(Constants.EnvAdminKey)))
                return BadRequest();

            var sessions = await _sessionizeService.FetchSessionsAsync();

            return Ok(sessions);
        }

        [HttpGet("api/admin/sessions")]
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