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
        public EventDetails EventDetails { get; private set; }
        public EventDataReaderService()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\json\\ga-greece-2020.json");
            var json = File.ReadAllText(filePath);
            this.EventDetails = JsonConvert.DeserializeObject<EventDetails>(json);
            HydrateSpeakers(this.EventDetails.Speakers, this.EventDetails.Schedule.SlotA);
            HydrateSpeakers(this.EventDetails.Speakers, this.EventDetails.Schedule.SlotB);
            HydrateSpeakers(this.EventDetails.Speakers, this.EventDetails.Schedule.SlotC);
        }

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