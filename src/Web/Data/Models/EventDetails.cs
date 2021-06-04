using System.Collections.Generic;

namespace EventManagement.Web.Data.Models
{
    public class EventDetails
    {
        public State State { get; set; }
        public Configuration Configuration { get; set; }
        public IList<Speaker> Speakers { get; set; }
        public IList<Volunteer> Volunteers { get; set; }
        public IList<Entity> Sponsors { get; set; }
        public IList<Entity> Communities { get; set; }
        public Schedule Schedule { get; set; }
        public IList<string> SpeakerOrder { get; set; }
        public IList<StreamUrl> StreamUrls { get; set; }
    }
}