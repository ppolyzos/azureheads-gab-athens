﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManagement.Web.Data.Models;
using EventManagement.Web.Services.Storage;
using EventManagement.Web.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace EventManagement.Web.Services.Events
{
    public interface IEventDetailsService
    {
        Task<EventDetails> FetchAsync(string container, string configFile);
    }

    public class EventDetailsService : IEventDetailsService
    {
        private readonly ILogger<EventDetailsService> _logger;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IMemoryCache _memoryCache;

        public EventDetailsService(
            ILogger<EventDetailsService> logger,
            IBlobStorageService blobStorageService,
            IMemoryCache memoryCache)
        {
            _logger = logger;
            _blobStorageService = blobStorageService;
            _memoryCache = memoryCache;
        }

        public async Task<EventDetails> FetchAsync(string container, string configFile)
        {
            if (_memoryCache.TryGetValue(Constants.CacheEventDetailsKey, out EventDetails eventDetails))
                return eventDetails;

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(3));

            eventDetails = await GetAsync(container, configFile);
            _memoryCache.Set(Constants.CacheEventDetailsKey, eventDetails, cacheEntryOptions);

            return eventDetails;
        }

        private async Task<EventDetails> GetAsync(string container, string configFile)
        {
            var eventDetails = await _blobStorageService.FetchAsync<EventDetails>(container, configFile);

            eventDetails.Schedule.Slots = eventDetails.Schedule.Sessions
                .Where(c => !string.IsNullOrEmpty(c.Room))
                .GroupBy(c => c.Room)
                .ToDictionary(g => g.Key, g => g.ToArray());

            foreach (var slot in eventDetails.Schedule.Slots)
            {
                HydrateSpeakers(eventDetails.Speakers, slot.Value);
            }
            
            _logger.LogInformation("event details loaded from storage service");

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