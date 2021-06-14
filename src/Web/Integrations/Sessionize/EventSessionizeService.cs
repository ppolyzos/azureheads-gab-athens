using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManagement.Api.Core.Infrastructure.Extensions;
using EventManagement.Web.Data.Models;
using EventManagement.Web.Utilities;
using Microsoft.Extensions.Caching.Memory;

namespace EventManagement.Web.Integrations.Sessionize
{
    public interface IEventSessionizeService
    {
        Task<IEnumerable<Speaker>> FetchSpeakersAsync();
        Task<IEnumerable<Session>> FetchSessionsAsync();
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
                .SetSlidingExpiration(TimeSpan.FromHours(1));

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

        public async Task<IEnumerable<Session>> FetchSessionsAsync()
        {
            if (_memoryCache.TryGetValue(Constants.CacheSessionsKey, out List<Session> sessions))
                return sessions;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1));

            var sessionizeSessions = await _sessionizeService.FetchSessionsAsync();

            var speakers = (await FetchSpeakersAsync()).ToArray();
            sessions = sessionizeSessions.Select(ss => new Session
            {
                Title = ss.Title,
                Description = ss.Description,
                SpeakerIds = ss.Speakers.Select(c => c.Name).ToArray(),
                Time = $"{ss.StartsAt.ToTimeZone("HH:mm")} - {ss.EndsAt.ToTimeZone("HH:mm")}",
                Room = ss.Room,
                Categories = ss.Categories.ToDictionary(c => c.Name,
                    sc => sc.CategoryItems.Select(si => si.Name))
            }).ToList();

            foreach (var session in sessions)
            {
                session.Speakers = new List<Speaker>();
                foreach (var speakerId in session.SpeakerIds)
                {
                    var speaker = speakers.FirstOrDefault(s => s.Name == speakerId);
                    if (speaker == null) continue;
                    
                    session.Speakers.Add(speaker);
                }
                session.SpeakerIds = session.Speakers.Select(c => c.Id).ToArray();
            }

            var slots = sessions.GroupBy(c => c.Room).Select(c => c.Key);
            foreach (var slot in slots)
            {
                var serviceSessions = GetServiceSessions(slot);
                if (slot.Equals("Slot 1"))
                {
                    serviceSessions = serviceSessions.Where(s => s.Title != "Welcome Keynote").ToArray();
                }
                sessions.AddRange(serviceSessions);
            }

            sessions = sessions.OrderBy(c => c.Room).ThenBy(c => c.Time).ToList();

            _memoryCache.Set(Constants.CacheSessionsKey, sessions, cacheEntryOptions);

            return sessions;
        }

        private Session[] GetServiceSessions(string room)
        {
            return new[]
            {
                new Session
                {
                    Title = "Welcome Keynote",
                    Time = "11:00 - 11:15",
                    Icon = "fa fa-building-o",
                    Room = room,
                    IsGreeting = true,
                },

                new Session
                {
                    Title = "Lunch Break",
                    Time = "14:00 - 15:00",
                    Icon = "fa fa-coffee",
                    Room = room,
                    IsGreeting = true
                },
                
                new Session
                {
                    Title = "Closing / Gifts",
                    Description = "Gifts are a kind offer from InfoQuest Technologies.",
                    Time = "17:45 - 18:00",
                    Icon = "fa fa-gift",
                    Room = room,
                    IsGreeting = true
                }
            };
        }
    }
}