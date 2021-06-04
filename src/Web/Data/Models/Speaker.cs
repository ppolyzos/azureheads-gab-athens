using System.Collections.Generic;

namespace EventManagement.Web.Data.Models
{
    public class Speaker
    {
        public string Id { get; set; }
        public string[] Aliases { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string About { get; set; }
        public string ImageUrl { get; set; }

        public IList<Link> Links { get; set; }
    }
}