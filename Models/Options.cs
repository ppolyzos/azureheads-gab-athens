using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gab_athens.Models
{
    public class HomepageOptions
    {
        public string Tickets { get; set; }
        public string Video { get; set; }

        public Speaker[] Speakers { get; set; }
    }

    public class Speaker
    {
        public string Name { get; set; }
        public string Job { get; set; }
        public string About { get; set; }

        public Social Social { get; set; }
    }

    public class Social
    {
        public string Website { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Linkedin { get; set; }
    }
}
