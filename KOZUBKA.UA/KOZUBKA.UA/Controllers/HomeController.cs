
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ua.kozubka.context.Models;
using ua.kozubka.context.Services.Repositories.MailRepository;
using ua.kozubka.context.Services.Repositories.ModelRepository;


namespace ua.kozubka.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISettingRepository _settingRepository;
        private readonly ICaptchaValidator _captchaValidator;
        private readonly UserRepository _userRepository;
        private readonly IFeedBackRepository _feedBackRepository;
        private readonly IMailRepository _mailRepository;

        public HomeController(ILogger<HomeController> logger,
            ISettingRepository settingRepository,
            ICaptchaValidator captchaValidator,
            UserRepository userRepository,
            IFeedBackRepository feedBackRepository,
            IMailRepository mailRepository)
        {
            _logger = logger;
            _settingRepository = settingRepository;
            _captchaValidator = captchaValidator;
            _userRepository = userRepository;
            _feedBackRepository = feedBackRepository;
            _mailRepository = mailRepository;
        }

        public IActionResult Index()
        {
            return View(); 
        }
        public IActionResult Index2()
        {
            return View("~/Views/Home/Neeon/Index.cshtml");
        }

        [HttpGet("terms", Name = "terms")]
        public IActionResult Terms()
        {
            return View();
        }
        [HttpGet("privacy", Name = "privacy")]
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet("contact",Name ="Contact")]
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost("contact", Name = "Contact")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contact(string captcha, string feedbackname, string feedbacktel, string feedbackemail, string feedbackmessage)
        {
            string redirectUrl;
            string googleKey = _settingRepository.GetSetting(context.Classes.Constants.SettingsOptions.GoogleReCaptchaSecretKey);
            _captchaValidator.UpdateSecretKey(googleKey);
            if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
            {
                ViewBag.Message = "Ми рахуємо що ви спамер ;( сумно.";
                redirectUrl = Url.Action("Contact", "Home");
                Response.Headers.Append("Refresh", $"2;url={redirectUrl}");
                return View("~/Views/Home/Messages.cshtml");
            }
            //Запись в таблицу 
            var currentUser = await _userRepository.GetUserAsync(User);
            FeedBack feedBack = new FeedBack
            {
                ActionTime = DateTime.Now,
                Email = feedbackemail,
                Message = feedbackmessage,
                Phone = feedbacktel,
                User = currentUser,
                UserName = feedbackname
            };
            await _feedBackRepository.AddFeedBack(feedBack);

            //Отправка почты
            await _mailRepository.SendFeedBack(feedBack);
            //Конец отправка почты
            ViewBag.Message = "Дякуємо за ваше повідомлення, ми зв'яжемось з вами, якщо це буде потрібно.";
            redirectUrl = Url.Action("Index", "Home");
            Response.Headers.Append("Refresh", $"3;url={redirectUrl}");
            return View("~/Views/Home/Messages.cshtml");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
