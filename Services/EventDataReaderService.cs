using System.Collections.Generic;
using System.IO;
using System.Linq;
using gab_athens.Models;
using Newtonsoft.Json;

namespace gab_athens.Services
{
    public class EventDataReaderService
    {
        public EventDetails Read()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\json\\ga-greece-2020.json");
            var json = File.ReadAllText(filePath);

            var eventDetails = JsonConvert.DeserializeObject<EventDetails>(json);
            this.HydrateSpeakers(eventDetails.Speakers, eventDetails.Schedule.SlotA);
            this.HydrateSpeakers(eventDetails.Speakers, eventDetails.Schedule.SlotB);
            this.HydrateSpeakers(eventDetails.Speakers, eventDetails.Schedule.SlotC);

            return eventDetails;
        }

        private void HydrateSpeakers(IList<Speaker> speakers, IEnumerable<Session> sessions)
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