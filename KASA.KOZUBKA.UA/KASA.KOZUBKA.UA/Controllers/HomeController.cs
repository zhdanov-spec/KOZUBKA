using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ua.kozubka.context.Models;

namespace ua.kozubka.kasa.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if(User.Identity!=null)
                if (User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Shop");
            return View();
        }
        [HttpGet("faq", Name = "FAQ")]

        public IActionResult FAQ()
        {
            return View();
        }
        [HttpGet("contact", Name = "Contact")]
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
