using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using gab_athens.Models;
using gab_athens.Services;
using gab_athens.Services.Storage;
using gab_athens.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace gab_athens.Controllers
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

        public async Task<IActionResult> Index(string speaker)
        {
            var eventContainer = Environment.GetEnvironmentVariable(Constants.EnvEventContainer) ?? "gab-events";
            var eventFile = Environment.GetEnvironmentVariable(Constants.EnvEventFile) ?? "ga-greece-2021_up.json";

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


            //return View("~/Views/Home/Index.cshtml"); // Default for gab-athens-2019
            // return View("~/Views/Ai/Index.cshtml"); // Default for ai-athens-2019
            return View("~/Views/ga/Index.cshtml", eventDetails);
        }

        [Route("speaker/{speaker}")]
        public async Task<IActionResult> Speaker(string speaker)
        {
            var eventDetails =
                await _eventDataStorageService.FetchEventDetailsAsync("gab-events", "ga-greece-2021.json");
            var card = eventDetails.Speakers.FirstOrDefault(c => c.Aliases.Contains(speaker.ToLowerInvariant()));
            ViewData["card"] = card;

            return View("~/Views/ga/Speaker.cshtml", eventDetails);
        }

        [Route("{speaker}")]
        public async Task<IActionResult> SpeakerIndex(string speaker)
        {
            return await Speaker(speaker);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}