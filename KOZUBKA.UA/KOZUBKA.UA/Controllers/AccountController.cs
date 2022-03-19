using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ua.kozubka.context.Classes;
using ua.kozubka.context.Models;
using ua.kozubka.context.Services.Repositories.Messangers.Telegram;
using ua.kozubka.context.Services.Repositories.ModelRepository;
using ua.kozubka.context.ViewModels.AccountViewModels;

namespace ua.kozubka.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly ICaptchaValidator _captchaValidator;
        private readonly ISettingRepository _settingRepository;
        private readonly ITelegramBotRepository _telegramBotRepository;
        private readonly string _googleReCaptchaSecretKey;
        public AccountController(UserRepository userRepository,
             ICaptchaValidator captchaValidator,
             ISettingRepository settingRepository,
             ITelegramBotRepository telegramBotRepository)
        {
            _userRepository = userRepository;
            _captchaValidator = captchaValidator;
            _settingRepository = settingRepository;
            _telegramBotRepository = telegramBotRepository;
            _googleReCaptchaSecretKey = settingRepository.Settings.Where(d => d.Key == Constants.SettingsOptions.GoogleReCaptchaSecretKey.ToString()).Select(d => d.Value).FirstOrDefault();
            _captchaValidator.UpdateSecretKey(_googleReCaptchaSecretKey);
        }
        #region Login Function
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallBack", "Account", new { ReturnUrl = returnUrl });
            var properites = _userRepository.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properites);
        }
        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            AccountViewModel accountViewModel = new AccountViewModel
            {
                LoginViewModel = new LoginViewModel
                {

                },
                RegisterViewModel = new RegisterViewModel
                {

                },

                ReturnUrl = returnUrl,
                ExternalLogins = (await _userRepository.GetExternalAuthenticationSchemes()).ToList()

            };
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Помилка авторізації: {remoteError}");
                return View("Login", accountViewModel);
            }
            var info = await _userRepository.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Помилка підключення до соціального профілю");
                return View("Login", accountViewModel);
            }
            var signInResult = await _userRepository.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true, true);
            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    var user = await _userRepository.FindByEmailAsync(email);
                    if (user == null)
                    {


                        user = new User
                        {
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            UName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                            ULastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Registred = DateTime.Now
                        };

                        return View(nameof(FinishSocialRegistration));
                    }


                    await _userRepository.AddLoginAsync(user, info);
                    await _userRepository.SignInAsync(user, true);
                    return LocalRedirect(returnUrl);

                }
                ViewBag.Message = "Ми не змогли отримати ваші данні, спробуйте звичайну реєстрацію";
                var redirectUrl1 = Url.Action("Index", "Home");
                Response.Headers.Append("Refresh", $"3;url={redirectUrl1}");
                return View("~/Views/Home/Messages.cshtml");
            }

        }
        public IActionResult FinishSocialRegistration()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Message = "Ви вже авторізовані";
                var redirectUrl1 = Url.Action("Index", "Home");
                Response.Headers.Append("Refresh", $"3;url={redirectUrl1}");
                return View("~/Views/Home/Messages.cshtml");
            }
            return View("~/Views/Home/FinishSocialRegistration.cshtml");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinishSocialRegistration(string phone, string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    ViewBag.Message = "Зачекайте";
                    Response.Headers.Append("Refresh", $"1;url={returnUrl}");
                    return View("~/Views/Home/Messages.cshtml");
                }
                else
                {
                    ViewBag.Message = "Ви вже авторізовані";
                    var redirectUrl1 = Url.Action("Index", "Home");
                    Response.Headers.Append("Refresh", $"3;url={redirectUrl1}");
                    return View("~/Views/Home/Messages.cshtml");

                }
            }
            Match match = Regex.Match(phone, @"\+38\ \(\d{3}\)\ \d{3}-\d{4}");
            if (!match.Success)
            {
                ModelState.AddModelError(string.Empty, "Невірний формат телефона.");
                return View(nameof(FinishSocialRegistration));
            }

            var info = await _userRepository.GetExternalLoginInfoAsync();
            User user = new User
            {
                Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                UName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                ULastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                Registred = DateTime.Now,
                PhoneNumber = phone
            };
            var createUserResult = await _userRepository.CreateAsync(user);
            if (!createUserResult.Succeeded)
            {

                createUserResult.SetModelErrors(ModelState);

                return View(nameof(FinishSocialRegistration));

            }
            await _userRepository.AddSubscribe(user.Email);
            _telegramBotRepository.SendNewRegistrationUser(user.UName, false);
            
            await _userRepository.AddLoginAsync(user, info);
            await _userRepository.SignInAsync(user, true);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    ViewBag.Message = "Зачекайте";
                    Response.Headers.Append("Refresh", $"1;url={returnUrl}");
                    return View("~/Views/Home/Messages.cshtml");
                }
                else
                {
                    ViewBag.Message = "Ви вже авторізовані";
                    var redirectUrl1 = Url.Action("Index", "Home");
                    Response.Headers.Append("Refresh", $"3;url={redirectUrl1}");
                    return View("~/Views/Home/Messages.cshtml");

                }
            }
            else
            {
                AccountViewModel accountViewModel = new AccountViewModel
                {
                    LoginViewModel = new LoginViewModel
                    {
                    },
                    RegisterViewModel = new RegisterViewModel
                    {

                    },
                    ReturnUrl = returnUrl,
                    ExternalLogins = (await _userRepository.GetExternalAuthenticationSchemes()).ToList()

                };

                return View(accountViewModel);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountViewModel model, string captcha)
        {

            if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
            {
                ModelState.AddModelError(string.Empty, "Ми рахуємо що ви спамер ;( сумно.");
                model.ExternalLogins = (await _userRepository.GetExternalAuthenticationSchemes()).ToList();
                return View(nameof(Login), model);
            }
            model.LoginViewModel.RememberMe = true;
            var result = await _userRepository.LogIn(model.LoginViewModel.Email, model.LoginViewModel.PhoneNumber, model.LoginViewModel.Password, model.LoginViewModel.RememberMe);
            if (!result.Succeeded)
            {
                model.ExternalLogins = await _userRepository.GetExternalAuthenticationSchemes();
                result.SetModelErrors(ModelState);
                return View(model);

            }
            // проверяем, принадлежит ли URL приложению
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }
            else if(string.IsNullOrEmpty(model.ReturnUrl))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectPermanent(model.ReturnUrl);
              
            }
        }
        #endregion
        #region Logout Function
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await _userRepository.LogOut();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else if (string.IsNullOrEmpty(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectPermanent(returnUrl);

            }
            
        }
        #endregion
        #region Reset Password
        public async Task<IActionResult> Recovery()
        {
            var user = await _userRepository.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.Message = "Ви авторізовані!";
                var redirectUrl1 = Url.Action("Index", "Home");
                Response.Headers.Append("Refresh", $"3;url={redirectUrl1}");
                return View("~/Views/Home/Messages.cshtml");
            }
            return View();
        }

        #endregion
        #region Register Function
        public async Task<IActionResult> Register(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    ViewBag.Message = "Зачекайте";
                    Response.Headers.Append("Refresh", $"1;url={returnUrl}");
                    return View("~/Views/Home/Messages.cshtml");
                }
                else
                {
                    ViewBag.Message = "Ви вже авторізовані";
                    var redirectUrl1 = Url.Action("Index", "Home");
                    Response.Headers.Append("Refresh", $"3;url={redirectUrl1}");
                    return View("~/Views/Home/Messages.cshtml");

                }
            }
            else
            {
                AccountViewModel accountViewModel = new AccountViewModel
                {
                    LoginViewModel = new LoginViewModel
                    {
                    },
                    RegisterViewModel = new RegisterViewModel
                    {

                    },
                    ReturnUrl = returnUrl,
                    ExternalLogins = (await _userRepository.GetExternalAuthenticationSchemes()).ToList()

                };

                return View(accountViewModel);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AccountViewModel model, string captcha)
        {
            if (string.IsNullOrEmpty(model.RegisterViewModel.Email))
                model.RegisterViewModel.Email = string.Format("{0}@pikapon.com", new string(model.RegisterViewModel.PhoneNumber.Where(char.IsDigit).ToArray()));
            bool isValidate = true;
            if (string.IsNullOrEmpty(model.RegisterViewModel.UName))
            {
                isValidate = false;
                ModelState.AddModelError(string.Empty, "Ви не заповнили поле Ім'я");
            }
            if (string.IsNullOrEmpty(model.RegisterViewModel.PhoneNumber))
            {
                isValidate = false;
                ModelState.AddModelError(string.Empty, "Ви не заповнили поле Телефон");
            }
            Match match = Regex.Match(model.RegisterViewModel.PhoneNumber, @"\+38\ \(\d{3}\)\ \d{3}-\d{4}");
            if (!match.Success)
            {
                isValidate = false;
                ModelState.AddModelError(string.Empty, "Невірний формат телефона.");
            }
            if (string.IsNullOrEmpty(model.RegisterViewModel.Password))
            {
                isValidate = false;
                ModelState.AddModelError(string.Empty, "Ви не заповнили поле Пароль");
            }
            if (!isValidate)
            {
                model.Page = "register";
                model.ExternalLogins = (await _userRepository.GetExternalAuthenticationSchemes()).ToList();
                return View(nameof(Register), model);
            }
            if (!await _captchaValidator.IsCaptchaPassedAsync(captcha))
            {
                ModelState.AddModelError(string.Empty, "Ми рахуємо що ви спамер ;( сумно.");
                model.Page = "register";
                return View(nameof(Register), model);
            }
            var createUserResult = await _userRepository.CheckIn(
               email: model.RegisterViewModel.Email,
               password: model.RegisterViewModel.Password,
               uName: model.RegisterViewModel.UName,
               username: model.RegisterViewModel.Email,
               phone: model.RegisterViewModel.PhoneNumber,
               viberUserId: null
               );
            if (!createUserResult.Succeeded)
            {

                createUserResult.SetModelErrors(ModelState);
                model.Page = "register";
                model.ExternalLogins = (await _userRepository.GetExternalAuthenticationSchemes()).ToList();
                return View(nameof(Register), model);

            }
            _telegramBotRepository.SendNewRegistrationUser(model.RegisterViewModel.UName, false);
            if (!model.RegisterViewModel.Email.Contains("@pikapon.com"))
            {
                await _userRepository.AddSubscribe(model.RegisterViewModel.Email);
            }
            if (!string.IsNullOrEmpty(model.ReturnUrl))
                return Redirect(model.ReturnUrl);
            else
                return RedirectToAction("Index", "Home");
        }
        #endregion
        #region AccessDenied Function
        public IActionResult AccessDenied(string returnUrl=null)
        {
            AccountViewModel model = new AccountViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }
        #endregion
    }
}
