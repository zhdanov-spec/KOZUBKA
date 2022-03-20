using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ua.kozubka.kasa.Controllers
{
    [Authorize]
    public class SmenaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
