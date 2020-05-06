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

    public class Entity
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string Width { get; set; }
        public string Style { get; set; }
        public bool Enabled { get; set; }
    }
    

    public class Link
    {
        public string Url { get; set; }
        public string Icon { get; set; }
    }

    public class EventDetails
    {
        public IList<Speaker> Speakers { get; set; }
        public IList<Entity> Sponsors { get; set; }
        public IList<Entity> Communities { get; set; }
    }
}