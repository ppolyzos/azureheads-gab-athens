using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EventManagement.Web.Data.Dtos;
using EventManagement.Web.Data.Models;
using EventManagement.Web.Integrations.Sessionize;
using EventManagement.Web.Services.Storage;
using EventManagement.Web.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventDataStorageService _eventDataStorageService;
        private readonly IEventSessionizeService _eventSessionizeService;

        public HomeController(IEventDataStorageService eventDataStorageService,
            IEventSessionizeService eventSessionizeService)
        {
            _eventDataStorageService = eventDataStorageService;
            _eventSessionizeService = eventSessionizeService;
        }

        public async Task<IActionResult> Index()
        {
            var eventContainer = Environment.GetEnvironmentVariable(Constants.EnvEventContainer) ?? "gab-events";
            var eventFile = Environment.GetEnvironmentVariable(Constants.EnvEventFile) ?? "ga-greece-2021.json";

            var eventDetails =
                await _eventDataStorageService.FetchEventDetailsAsync(eventContainer, eventFile);

            if (eventDetails.SpeakerOrder != null)
            {
                var speakers = (await _eventSessionizeService.FetchSpeakersAsync()).ToArray();
                eventDetails.Speakers = eventDetails.SpeakerOrder
                    .Select(key => speakers.FirstOrDefault(c => c.Id == key))
                    .Where(s => s != null)
                    .ToList();
            }
            
            eventDetails.Schedule = new Schedule { Sessions = await _eventSessionizeService.FetchSessionsAsync() };
            eventDetails.Schedule.Slots = eventDetails.Schedule.Sessions
                .Where(c => !string.IsNullOrEmpty(c.Room))
                .OrderBy(c => c.Room)
                .GroupBy(c => c.Room)
                .ToDictionary(g => g.Key, g => g.ToArray());
            
            foreach (var slot in eventDetails.Schedule.Slots)
            {
                var streamUrls =
                    eventDetails.State.ShowStreamUrlsFrom <= DateTime.UtcNow &&
                    eventDetails.State.ShowStreamUrlsTo > DateTime.UtcNow
                        ? eventDetails.StreamUrls
                        : null;
                Hydrate(slot.Value, eventDetails.Speakers, streamUrls);
            }

            //return View("~/Views/Home/Index.cshtml"); // Default for EventManagement.Web-2019
            // return View("~/Views/Ai/Index.cshtml"); // Default for ai-athens-2019
            return View("~/Views/ga/Index.cshtml", eventDetails);
        }
        
        public IActionResult Error() => View(new ErrorDto { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        private static void Hydrate(IEnumerable<Session> sessions, IList<Speaker> speakers, IList<StreamUrl> streamUrls)
        {
            foreach (var session in sessions)
            {
                session.Speakers = new List<Speaker>();
                session.StreamUrl = streamUrls?.FirstOrDefault(c => c.Name.Equals(session.Room))?.Url;

                if (session.Title == "Welcome Keynote" || session.Title == "Closing / Gifts")
                {
                    session.StreamUrl = streamUrls?.FirstOrDefault(c => c.Name.Equals("Slot 1"))?.Url;
                }
                
                if (session.SpeakerIds == null) continue;

                foreach (var speakerId in session.SpeakerIds)
                {
                    var speaker = speakers.FirstOrDefault(c => c.Id.Equals(speakerId));
                    if (speaker == null) continue;

                    session.Speakers.Add(speaker);
                }
            }
        }
    }
}