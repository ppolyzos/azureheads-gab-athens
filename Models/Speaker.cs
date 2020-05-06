﻿using System.Collections.Generic;
using Newtonsoft.Json;

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

    public class Schedule
    {
        public IList<Session> SlotA { get; set; }
        public IList<Session> SlotB { get; set; }
        public IList<Session> SlotC { get; set; }
    }

    public class Session
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string[] SpeakerIds { get; set; }
        
        [JsonIgnore]
        public IList<Speaker> Speakers { get; set; }
        public string Icon { get; set; }
        public string Time { get; set; }
        public string VideoUrl { get; set; }
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
        public Schedule Schedule { get; set; }
    }
}