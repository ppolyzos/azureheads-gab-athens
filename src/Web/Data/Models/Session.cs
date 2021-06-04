using System.Collections.Generic;
using Newtonsoft.Json;

namespace EventManagement.Web.Data.Models
{
    public class Session
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] SpeakerIds { get; set; }
        public Dictionary<string, IEnumerable<string>> Categories { get; set; }
        
        [JsonIgnore]
        public IList<Speaker> Speakers { get; set; }
        public string Icon { get; set; }
        public string Time { get; set; }
        public string VideoUrl { get; set; }
        public string StreamUrl { get; set; }
        public bool IsGreeting { get; set; }
        public string Room { get; set; }
    }
}