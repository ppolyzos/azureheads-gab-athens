using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using Microsoft.AspNetCore.Mvc;
using gab_athens.Models;

namespace gab_athens.Controllers
{
    public class HomeController : Controller
    {
        private Dictionary<string, string> mappings = new Dictionary<string, string>()
        {
            { "chatzipavlis", "antonios-chatzipavlis" } ,
            { "fintzos", "dimitris-fintzos" } ,
            { "pantazis", "dimitris-pantazis" } ,
            { "grammatikos", "george-grammatikos" } ,
            { "markou", "george-markou" } ,
            { "kalyva", "georgia-kalyva" } ,
            { "kavvalakis", "giorgos-kavvalakis" } ,
            { "pantos", "konstantinos-pantos" } ,
            { "haggar", "michail-haggar" } ,
            { "antoniou", "nikolaos-antoniou" } ,
            { "apostolidis", "pantelis-apostolidis" } ,
            { "polyzos", "paris-polyzos" } ,
            { "kappas", "vaggelis-kappas" } ,
            { "touliatos", "yanni-touliatos" },
            { "ioannidis", "vassilis-ioannidis" },
            { "spyrou", "george-spyrou" },
            { "ziazios", "konstantinos-ziazios" },
            { "nikolaidis", "michalis-nikolaidis" },
            { "set1", "set-1" },
            { "set2", "set-2" },
            { "set3", "set-3" },
            { "set4", "set-4" },
        };

        public IActionResult Index(string speaker)
        {
            //return View("~/Views/Home/Index.cshtml"); // Default for gab-athens-2019
            //return View("~/Views/Ai/Index.cshtml"); // Default for ai-athens-2019
            return View("~/Views/ga-2020/Index.cshtml"); // Default for global-azure-2020
        }

        [Route("speaker/{speaker}")]
        public IActionResult Speaker(string speaker)
        {
            speaker = speaker.ToLowerInvariant();
            mappings.TryGetValue(speaker, out var speakerImage);
            var speakerCard = new ExpandoObject();
            speakerCard.TryAdd("image", speakerImage ?? "speakers");
            return View("~/Views/ga-2020/Speaker.cshtml", speakerCard);
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
