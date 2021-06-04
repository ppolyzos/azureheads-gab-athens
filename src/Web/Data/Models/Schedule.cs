using System.Collections.Generic;

namespace EventManagement.Web.Data.Models
{
    public class Schedule
    {
        public Dictionary<string, Session[]> Slots { get; set; }
        public IEnumerable<Session> Sessions { get; set; }
    }
}