using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using gab_athens.Models;
using gab_athens.Services;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace gab_athens.Controllers
{
    public class HomeController : Controller
    {
        private readonly EventDataReaderService _eventDataReaderService;

        public HomeController(EventDataReaderService eventDataReaderService)
        {
            _eventDataReaderService = eventDataReaderService;
        }

        public IActionResult Index(string speaker)
        {
            var eventDetails = _eventDataReaderService.Read();
            //return View("~/Views/Home/Index.cshtml"); // Default for gab-athens-2019
            // return View("~/Views/Ai/Index.cshtml"); // Default for ai-athens-2019
            return View("~/Views/ga-2020/Index.cshtml", eventDetails); // Default for global-azure-2020
        }

        [Route("speaker/{speaker}")]
        public IActionResult Speaker(string speaker)
        {
            var eventDetails = _eventDataReaderService.Read();
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
