using System.Collections.Generic;
using System.IO;
using gab_athens.Models;
using Newtonsoft.Json;

namespace gab_athens.Services
{
    public class EventDataReaderService
    {
        public EventDetails Read()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\json\\ga-greece-2020.json");
            var json = File.ReadAllText(filePath);

            return JsonConvert.DeserializeObject<EventDetails>(json);
        }
    }
}