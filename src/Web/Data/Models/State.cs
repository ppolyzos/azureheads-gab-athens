using System;

namespace EventManagement.Web.Data.Models
{
    public class State
    {
        public bool ShowRegistration { get; set; }
        public bool ShowSchedule { get; set; }
        public bool ShowSessionLinks { get; set; }
        public DateTime? ShowStreamUrlsFrom { get; set; }
        public DateTime? ShowStreamUrlsTo { get; set; }
        public bool ShowSessions { get; set; }
        public bool ShowSpeakers { get; set; }
        public bool ShowVolunteers { get; set; }
        public bool ShowSponsors { get; set; }
        public bool ShowSpeakerDescription { get; set; }
    }
}