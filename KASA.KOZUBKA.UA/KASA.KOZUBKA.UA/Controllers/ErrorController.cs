using Microsoft.AspNetCore.Mvc;

namespace ua.kozubka.kasa.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorTitle = "Сторінка не знайдена";
                    ViewBag.ErrorMessage = "Вибачте, але цієї сторінки вже не існує";
                    break;
                default:
                    ViewBag.ErrorTitle = "Щось не так";
                    ViewBag.ErrorMessage = string.Format("Вибачте, виникла помилка - {0}", statusCode);
                    break;
            }
            ViewBag.ErrorCode = statusCode;
            return View("Error");
        }
    }
}
