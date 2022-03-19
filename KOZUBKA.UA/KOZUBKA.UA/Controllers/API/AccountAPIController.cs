using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ua.kozubka.context.Models;
using ua.kozubka.context.Services.Repositories.ModelRepository;
using ua.kozubka.context.Services.Repositories.Partitial;
using ua.kozubka.context.Services.Repositories.SMSReporsitory;
using ua.kozubka.context.ViewModels.APIViewModels;

namespace ua.kozubka.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountAPIController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly IPartialHelper _partialHelper;
        private readonly ISMSRepository _smsRepository;
        private readonly IESputnikRepository _eSputnikRepository;
        public AccountAPIController(UserRepository userRepository,
            IPartialHelper partialHelper,
            ISMSRepository smsRepository,
            IESputnikRepository eSputnikRepository)
        {
            _userRepository = userRepository;
            _partialHelper = partialHelper;
            _smsRepository = smsRepository;
            _eSputnikRepository = eSputnikRepository;
        }
        [HttpPost]
        [Route("subscribe")]
        public async Task<JsonResult> Subscribe([FromBody] AccountAPI account)
        {
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false,
                HtmlErrorText = "Помилка"
            };

            await _userRepository.AddSubscribe(account.Email);
            await _eSputnikRepository.AddContact(account.Email);
            model.HaveResult = true;
            model.HtmlErrorText = "Дякуємо за підписку";

            return Json(model);
        }
        [HttpPost]
        [Route("sendotp")]
        public async Task<JsonResult> SendOtp([FromBody] AccountAPI account)
        {
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false,
                HtmlErrorText = "Помилка"
            };
            var currentUser = await _userRepository.GetUserAsync(User);
            if (currentUser != null)
            {
                model.HtmlErrorText = "Ви вже авторізовані";
                return Json(model);
            }
            var user = await _userRepository.GetUserByPhoneOrEmail(account.Phone, null);
            if (user == null)
            {
                model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<string>("Partial/Account/_RecoveryPass", account.Phone);
                model.HaveResult = true;
                return Json(model);
            }
            TimeSpan diffSec = DateTime.Now - user.OTPSend;
            if (diffSec.TotalSeconds < 60)
            {
                model.HtmlErrorText = string.Format("Спробуйте через {0} секунд", (60 - diffSec.Seconds).ToString());
                return Json(model);
            }
            model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<string>("Partial/Account/_RecoveryPass", account.Phone);
            model.HaveResult = true;
            string otpPass = GenerateCode(4);
            string phoneToSend = Regex.Replace(user.PhoneNumber, "[^0-9]", "");
            await _userRepository.UpdateUserOTP(account.Phone, otpPass);
            //Тут мы должны отправить
            _smsRepository.SendSms(phoneToSend, string.Format("Код відновлення {0} дійсний 10 хвилин", otpPass));
            return Json(model);
        }
        [HttpPost]
        [Route("changepass")]
        public async Task<JsonResult> ChangePass([FromBody] AccountAPI account)
        {
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false,
                HtmlErrorText = "Помлка"
            };
            var currentUser = await _userRepository.GetUserAsync(User);
            if (currentUser != null)
            {
                model.HtmlErrorText = "Ви вже авторізовані";
                return Json(model);
            }
            var user = await _userRepository.GetUserByPhoneOrEmail(account.Phone, null);
            if (user == null)
            {
                model.HaveResult = true;
                return Json(model);
            }
            TimeSpan diffSec = DateTime.Now - user.OTPSend;
            if (diffSec.TotalMinutes > 10)
            {
                model.HtmlErrorText = "Код застарий, спробуйте отримати новий";
                return Json(model);
            }
            if (account.OTPCode != user.OTP)
            {
                model.HtmlErrorText = "Код не вірний";
                return Json(model);
            }
            string newPass = GenerateCode(6);
            var _passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<User>)) as IPasswordValidator<User>;
            var _passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<User>)) as IPasswordHasher<User>;
            user.PasswordHash = _passwordHasher.HashPassword(user, newPass);
            await _userRepository.UpdateAsync(user);
            string phoneToSend = Regex.Replace(user.PhoneNumber, "[^0-9]", "");
            await _userRepository.LogIn(null, user.PhoneNumber, newPass, true);
            _smsRepository.SendSms(phoneToSend, string.Format("Ваш новий пароль {0}", newPass));
            model.HaveResult = true;
            return Json(model);
        }
        #region Private Function
        private string GenerateCode(int length)
        {
            Random _rng = new Random();
            string _chars = "0123456789";
            char[] buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = _chars[_rng.Next(_chars.Length)];
            }
            return (new string(buffer)).ToLower();
        }
        #endregion
        public class AccountAPI
        {
            public string Phone { get; set; }
            public string OTPCode { get; set; }
            public string Email { get; set; }
        }
    }
}
