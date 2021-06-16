using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EventManagement.Web.Data.Dtos;
using EventManagement.Web.Services.Events;
using EventManagement.Web.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventService _eventService;

        public HomeController(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            var eventContainer = Environment.GetEnvironmentVariable(Constants.EnvEventContainer) ?? "gab-events";
            var eventFile = Environment.GetEnvironmentVariable(Constants.EnvEventFile) ?? "ga-greece-2021.json";

            var eventDetails = await _eventService.FetchAsync(eventContainer, eventFile);

            return View("~/Views/ga/Index.cshtml", eventDetails);
        }

        public IActionResult Error() => View(new ErrorDto
            { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}