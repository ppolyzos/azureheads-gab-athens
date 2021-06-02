using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManagement.Web.Models;
using EventManagement.Web.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace EventManagement.Web.Services.Storage
{
    public interface IEventDataStorageService
    {
        Task<EventDetails> FetchEventDetailsAsync(string container, string configFile);
    }

    public class EventDataStorageService : IEventDataStorageService
    {
        private readonly ILogger<EventDataStorageService> _logger;
        private readonly IStorageService _storageService;
        private readonly IMemoryCache _memoryCache;

        public EventDataStorageService(
            ILogger<EventDataStorageService> logger,
            IStorageService storageService,
            IMemoryCache memoryCache)
        {
            _logger = logger;
            _storageService = storageService;
            _memoryCache = memoryCache;
        }

        public async Task<EventDetails> FetchEventDetailsAsync(string container, string configFile)
        {
            if (_memoryCache.TryGetValue(Constants.CacheEventsKey, out EventDetails eventDetails))
                return eventDetails;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(3));

            eventDetails = await GetAsync(container, configFile);
            _memoryCache.Set(Constants.CacheEventsKey, eventDetails, cacheEntryOptions);

            return eventDetails;
        }

        private async Task<EventDetails> GetAsync(string container, string configFile)
        {
            var eventDetails = await _storageService.FetchAsync<EventDetails>(container, configFile);

            eventDetails.Schedule.Slots = eventDetails.Schedule.Sessions
                .Where(c => !string.IsNullOrEmpty(c.Room))
                .GroupBy(c => c.Room)
                .ToDictionary(g => g.Key, g => g.ToArray());

            foreach (var slot in eventDetails.Schedule.Slots)
            {
                HydrateSpeakers(eventDetails.Speakers, slot.Value);
            }
            
            _logger.LogInformation("event details loaded from storage service.");

            return eventDetails;
        }

        private static void HydrateSpeakers(IList<Speaker> speakers, IEnumerable<Session> sessions)
        {
            foreach (var session in sessions)
            {
                session.Speakers = new List<Speaker>();
                foreach (var speakerId in session.SpeakerIds)
                {
                    var speaker = speakers.FirstOrDefault(c => c.Id.Equals(speakerId));
                    if (speaker == null) continue;

                    session.Speakers.Add(speaker);
                }
            }
        }
    }
}