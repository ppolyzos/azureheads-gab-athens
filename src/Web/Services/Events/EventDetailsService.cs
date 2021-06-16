using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManagement.Web.Application.Cache.Enumerations;
using EventManagement.Web.Application.Cache.Services;
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
        private readonly IInMemoryCacheService _inMemoryCacheService;

        public EventDetailsService(
            ILogger<EventDetailsService> logger,
            IBlobStorageService blobStorageService,
            IInMemoryCacheService inMemoryCacheService)
        {
            _logger = logger;
            _blobStorageService = blobStorageService;
            _inMemoryCacheService = inMemoryCacheService;
        }

        public async Task<EventDetails> FetchAsync(string container, string configFile)
        {
            return await _inMemoryCacheService.CacheAsync(async () => await GetAsync(container, configFile),
                Constants.CacheEventDetailsKey, CacheDuration.CacheLow);
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