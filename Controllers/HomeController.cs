using System.Diagnostics;
using System.Linq;
using gab_athens.Models;
using gab_athens.Services;
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

        public IActionResult Index(string speaker)
        {
            //return View("~/Views/Home/Index.cshtml"); // Default for gab-athens-2019
            // return View("~/Views/Ai/Index.cshtml"); // Default for ai-athens-2019
            return View("~/Views/ga-2020/Index.cshtml",
                _eventDataReaderService.EventDetails); // Default for global-azure-2020
        }

        [Route("speaker/{speaker}")]
        public IActionResult Speaker(string speaker)
        {
            var eventDetails = _eventDataReaderService.EventDetails;
            var card = eventDetails.Speakers.FirstOrDefault(c => c.Aliases.Contains(speaker.ToLowerInvariant()));
            ViewData["card"] = card;

            return View("~/Views/ga-2020/Speaker.cshtml", eventDetails);
        }

        [Route("{speaker}")]
        public IActionResult SpeakerIndex(string speaker)
        {
            return Speaker(speaker);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}