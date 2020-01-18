using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using gab_athens.Models;

namespace gab_athens.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            //return View("~/Views/Home/Index.cshtml"); // Default for gab-athens-2019
            return View("~/Views/Ai/Index.cshtml"); // Default for ai-athens-2019
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
