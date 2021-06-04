using System.Collections.Generic;

namespace EventManagement.Web.Data.Models
{
    public class Configuration
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
        public string DateExt { get; set; }
        public string RegistrationUrl { get; set; }
        public string CallForSpeakersUrl { get; set; }
        public string CallForVolunteersUrl { get; set; }
        public string BgHeroCssName { get; set; }
        public string ProductionUrl { get; set; }
        public string ProductionImageUrl { get; set; }
        public IList<Link> Links { get; set; }
    }
}