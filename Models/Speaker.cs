using System.Collections.Generic;

namespace gab_athens.Models
{
    public class Speaker
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string About { get; set; }
        public string ImageUrl { get; set; }

        public IList<Link> Links { get; set; }
    }

    public class Link
    {
        public string Url { get; set; }
        public string Icon { get; set; }
    }

    public class EventDetails
    {
        public IList<Speaker> Speakers { get; set; }
    }
}