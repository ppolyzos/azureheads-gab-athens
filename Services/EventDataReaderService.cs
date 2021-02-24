using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using gab_athens.Models;
using Newtonsoft.Json;

namespace gab_athens.Services
{
    public interface IEventDataReaderService
    {
        public EventDetails EventDetails { get; }
    }

    public class EventDataReaderService : IEventDataReaderService
    {
        
        public EventDataReaderService()
        {
            var eventFile = Environment.GetEnvironmentVariable("EVENT_FILE") ?? "ga-greece-2021.json";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "json", eventFile);
            var json = File.ReadAllText(filePath);
            EventDetails = JsonConvert.DeserializeObject<EventDetails>(json);
            HydrateSpeakers(EventDetails.Speakers, EventDetails.Schedule.SlotA);
            HydrateSpeakers(EventDetails.Speakers, EventDetails.Schedule.SlotB);
            HydrateSpeakers(EventDetails.Speakers, EventDetails.Schedule.SlotC);
        }

        public EventDetails EventDetails { get; }

        private static void HydrateSpeakers(IList<Speaker> speakers, IEnumerable<Session> sessions)
        {
            foreach (var session in sessions)
            {
                session.Speakers = new List<Speaker>();
                foreach (var speakerId in session.SpeakerIds)
                {
                    var speaker = speakers.First(c => c.Id.Equals(speakerId));
                    session.Speakers.Add(speaker);
                }
            }
        }
    }
}