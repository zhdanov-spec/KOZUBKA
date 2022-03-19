using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Mvc;
using ua.kozubka.context.Models;
using ua.kozubka.context.Services.Repositories.MailRepository;
using ua.kozubka.context.Services.Repositories.ModelRepository;
using ua.kozubka.context.ViewModels.APIViewModels;

namespace ua.kozubka.cv.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CVAPIController : Controller
    {
        private readonly ISettingRepository _settingRepository;
        private readonly ICaptchaValidator _captchaValidator;
        private readonly IMailRepository _mailRepository;
        public CVAPIController(ISettingRepository settingRepository,
            ICaptchaValidator captchaValidator,
            IMailRepository mailRepository)
        {
            _settingRepository= settingRepository;
            _captchaValidator= captchaValidator;
            _mailRepository = mailRepository;
        }
        [HttpPost("sendemail", Name = "SendEmail")]
        
        public async Task<IActionResult> SendEmail([FromForm] CVAPI cv)
        {
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false,
                HtmlErrorText = "Помилка"
            };
            string googleKey = _settingRepository.GetSetting(context.Classes.Constants.SettingsOptions.GoogleReCaptchaSecretKey);
            _captchaValidator.UpdateSecretKey(googleKey);
            if(!await _captchaValidator.IsCaptchaPassedAsync(cv.captcha))
            {
                return Json(model);
            }
            
            //Отправка почты
            FeedBack feedBack = new FeedBack
            {
                ActionTime = DateTime.Now,
                Email = cv.email,
                Message = cv.message,
                Phone = "",
                User = null,
                UserName = cv.name
            };
            await _mailRepository.SendFeedBack(feedBack);
            model.HaveResult = true;
            
            return Json(model);
        }
        public class CVAPI
        {
            public string? captcha { get; set; }
            public string? name { get; set; }
            public string? subject { get; set; }
            public string? email { get; set; }
            public string? message { get; set; }
        }
        
    }
    
}
