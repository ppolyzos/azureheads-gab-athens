﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gab_athens.Models;
using gab_athens.Utilities;
using Microsoft.Extensions.Caching.Memory;

namespace gab_athens.Services.Storage
{
    public interface IEventDataStorageService
    {
        Task<EventDetails> FetchEventDetailsAsync(string container, string configFile);
    }

    public class EventDataStorageService : IEventDataStorageService
    {
        private readonly IStorageService _storageService;
        private readonly IMemoryCache _memoryCache;

        public EventDataStorageService(IStorageService storageService, IMemoryCache memoryCache)
        {
            _storageService = storageService;
            _memoryCache = memoryCache;
        }

        public async Task<EventDetails> FetchEventDetailsAsync(string container, string configFile)
        {
            if (_memoryCache.TryGetValue(Constants.CacheEventsKey, out EventDetails eventDetails)) return eventDetails;
            
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromDays(3));
                
            eventDetails = await GetAsync(container, configFile);
            _memoryCache.Set(Constants.CacheEventsKey, eventDetails, cacheEntryOptions);

            return eventDetails;
        }
        
        private async Task<EventDetails> GetAsync(string container, string configFile)
        {
            var eventDetails = await _storageService.FetchAsync<EventDetails>(container, configFile);

            HydrateSpeakers(eventDetails.Speakers, eventDetails.Schedule.SlotA);
            HydrateSpeakers(eventDetails.Speakers, eventDetails.Schedule.SlotB);
            HydrateSpeakers(eventDetails.Speakers, eventDetails.Schedule.SlotC);

            eventDetails.Schedule.Slots = new Dictionary<string, IList<Session>>()
            {
                { "Slot A", eventDetails.Schedule.SlotA },
                { "Slot B", eventDetails.Schedule.SlotB },
                { "Slot C", eventDetails.Schedule.SlotC }
            };

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