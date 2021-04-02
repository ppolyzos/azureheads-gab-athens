using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gab_athens.Models;
using gab_athens.Utilities;
using Microsoft.Extensions.Caching.Memory;

namespace gab_athens.Services
{
    public interface IEventSessionizeService
    {
        Task<IEnumerable<Speaker>> FetchSpeakersAsync();
    }

    public class EventSessionizeService : IEventSessionizeService
    {
        private readonly Dictionary<string, string> _icons = new Dictionary<string, string>
        {
            { "LinkedIn", "fa fa-linkedin" },
            { "Blog", "fa fa-globe" },
            { "Company_Website", "fa fa-globe" },
            { "Other", "fa fa-globe" },
            { "Facebook", "fa fa-facebook" },
            { "Twitter", "fa fa-twitter" },
            { "Instagram", "fa fa-instagram" }
        };

        private readonly ISessionizeService _sessionizeService;
        private readonly IMemoryCache _memoryCache;

        public EventSessionizeService(ISessionizeService sessionizeService, IMemoryCache memoryCache)
        {
            _sessionizeService = sessionizeService;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<Speaker>> FetchSpeakersAsync()
        {
            if (_memoryCache.TryGetValue(Constants.CacheSpeakersKey, out IList<Speaker> speakers))
                return speakers;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(1));

            var sessionizeSpeakers = await _sessionizeService.FetchSpeakersAsync();

            speakers = sessionizeSpeakers.Select(ss => new Speaker
            {
                Id = $"{ss.FirstName}-{ss.LastName}".ToLowerInvariant(),
                Aliases = new[] { ss.LastName.ToLowerInvariant() },
                Name = ss.FullName,
                Title = ss.TagLine,
                About = ss.Bio,
                ImageUrl = ss.ProfilePicture,
                Links = ss.Links.Select(c => new Link
                    { Url = c.Url, Icon = _icons[c.LinkType] }).ToList()
            }).ToList();

            _memoryCache.Set(Constants.CacheSpeakersKey, speakers, cacheEntryOptions);

            return speakers;
        }
    }
}