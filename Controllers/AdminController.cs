using System;
using System.Threading.Tasks;
using gab_athens.Services;
using gab_athens.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace gab_athens.Controllers
{
    public class AdminController : Controller
    {
        private readonly IEventDataReaderService _eventDataReaderService;
        private readonly IMemoryCache _memoryCache;

        public AdminController(IEventDataReaderService eventDataReaderService,
            IMemoryCache memoryCache)
        {
            _eventDataReaderService = eventDataReaderService;
            _memoryCache = memoryCache;
        }

        [HttpPost("api/cache/refresh")]
        public async Task<IActionResult> CacheRefreshAsync([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key) || !string.Equals(key, Environment.GetEnvironmentVariable(Constants.EnvAdminKey)))
                return BadRequest();
            
            _memoryCache.Remove(Constants.CacheEventsKey);
            
            var eventFile = Environment.GetEnvironmentVariable(Constants.EnvEventFile) ?? "ga-greece-2021.json";
            var eventContainer = Environment.GetEnvironmentVariable(Constants.EnvEventContainer) ?? "gab-events";

            var content = await _eventDataReaderService.FetchEventDetailsAsync(eventContainer, eventFile);
            return Ok(content);
        } 
        
        [HttpGet("api/current-event")]
        public async Task<IActionResult> GetCurrentEventAsync([FromQuery] string key)
        {
            if (string.IsNullOrEmpty(key) || !string.Equals(key, Environment.GetEnvironmentVariable(Constants.EnvAdminKey)))
                return BadRequest();
            
            _memoryCache.Remove(Constants.CacheEventsKey);
            
            var eventFile = Environment.GetEnvironmentVariable(Constants.EnvEventFile) ?? "ga-greece-2021.json";
            var eventContainer = Environment.GetEnvironmentVariable(Constants.EnvEventContainer) ?? "gab-events";

            var content = await _eventDataReaderService.FetchEventDetailsAsync(eventContainer, eventFile);
            return Ok(content);
        } 
    }
}