using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManagement.Web.Application.Cache;
using EventManagement.Web.Data.Models;
using EventManagement.Web.Integrations.Sessionize;
using EventManagement.Web.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace EventManagement.Web.Services.Events
{
    public interface IEventService
    {
        Task<EventDetails> FetchAsync(string container, string file);
    }
    
    public class EventService : IEventService
    {
        private readonly IEventDetailsService _eventDetailsService;
        private readonly IEventSessionizeService _eventSessionizeService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<EventService> _logger;

        public EventService(IEventDetailsService eventDetailsService,
            IEventSessionizeService eventSessionizeService,
            IMemoryCache memoryCache,
            ILogger<EventService> logger)
        {
            _eventDetailsService = eventDetailsService;
            _eventSessionizeService = eventSessionizeService;
            _memoryCache = memoryCache;
            _logger = logger;
        }
        
        public async Task<EventDetails> FetchAsync(string container, string file)
        {
            if (_memoryCache.TryGetValue(Constants.CacheEventsKey, out EventDetails eventDetails))
            {
                _logger.LogInformation("event loaded from cache");
                return eventDetails;
            }
            
            eventDetails =
                await _eventDetailsService.FetchAsync(container, file);
            _logger.LogInformation("event configuration has been loaded");

            if (eventDetails.SpeakerOrder != null)
            {
                var speakers = (await _eventSessionizeService.FetchSpeakersAsync()).ToArray();
                eventDetails.Speakers = eventDetails.SpeakerOrder
                    .Select(key => speakers.FirstOrDefault(c => c.Id == key))
                    .Where(s => s != null)
                    .ToList();
                _logger.LogInformation("speaker order has been applied");
            }
            
            eventDetails.Schedule = new Schedule { Sessions = await _eventSessionizeService.FetchSessionsAsync() };
            eventDetails.Schedule.Slots = eventDetails.Schedule.Sessions
                .Where(c => !string.IsNullOrEmpty(c.Room))
                .OrderBy(c => c.Room)
                .GroupBy(c => c.Room)
                .ToDictionary(g => g.Key, g => g.ToArray());
            _logger.LogInformation("schedule has been generated");
            
            foreach (var slot in eventDetails.Schedule.Slots)
            {
                var streamUrls =
                    eventDetails.State.ShowStreamUrlsFrom <= DateTime.UtcNow &&
                    eventDetails.State.ShowStreamUrlsTo > DateTime.UtcNow
                        ? eventDetails.StreamUrls
                        : null;
                Hydrate(slot.Value, eventDetails.Speakers, streamUrls);
            }
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(3));
            _memoryCache.Set(Constants.CacheEventsKey, eventDetails, cacheEntryOptions);
            _logger.LogInformation("event cached");

            return eventDetails;
        }
        
        private static void Hydrate(IEnumerable<Session> sessions, IList<Speaker> speakers, IList<StreamUrl> streamUrls)
        {
            foreach (var session in sessions)
            {
                session.Speakers = new List<Speaker>();
                session.StreamUrl = streamUrls?.FirstOrDefault(c => c.Name.Equals(session.Room))?.Url;

                if (session.Title is "Welcome Keynote" or "Closing / Gifts")
                {
                    session.StreamUrl = streamUrls?.FirstOrDefault(c => c.Name.Equals("Slot 1"))?.Url;
                }
                
                if (session.SpeakerIds == null) continue;

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