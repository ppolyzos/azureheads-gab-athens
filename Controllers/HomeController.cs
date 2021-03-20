using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using gab_athens.Models;
using gab_athens.Services;
using gab_athens.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace gab_athens.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventDataReaderService _eventDataReaderService;

        public HomeController(IEventDataReaderService eventDataReaderService)
        {
            _eventDataReaderService = eventDataReaderService;
        }

        public async Task<IActionResult> Index(string speaker)
        {
            var eventContainer = Environment.GetEnvironmentVariable(Constants.EnvEventContainer) ?? "gab-events";
            var eventFile = Environment.GetEnvironmentVariable(Constants.EnvEventFile) ?? "ga-greece-2021.json";
            
            var eventDetails =
                await _eventDataReaderService.FetchEventDetailsAsync(eventContainer, eventFile);
            //return View("~/Views/Home/Index.cshtml"); // Default for gab-athens-2019
            // return View("~/Views/Ai/Index.cshtml"); // Default for ai-athens-2019
            return View("~/Views/ga/Index.cshtml", eventDetails);
        }

        [Route("speaker/{speaker}")]
        public async Task<IActionResult> Speaker(string speaker)
        {
            var eventDetails =
                await _eventDataReaderService.FetchEventDetailsAsync("gab-events", "ga-greece-2021.json");
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