using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ua.kozubka.context.Classes;
using ua.kozubka.context.Models;
using ua.kozubka.context.Services.Repositories.ModelRepository;

namespace ua.kozubka.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserRepository _userRepository;
        public ProfileController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<IActionResult> Index()
        {
            var model = await _userRepository.GetUserAsync(User);
            if (model == null) return NotFound();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(User model, IFormFile profileImage)
        {
            var currentUser = await _userRepository.GetUserAsync(User);
            if (string.IsNullOrEmpty(model.UName))
            {
                ModelState.AddModelError(string.Empty, "Ім'я - обов'язкове");
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError(string.Empty, "Email - обов'язковий");
            }
            if (string.IsNullOrEmpty(model.PhoneNumber))
            {
                ModelState.AddModelError(string.Empty, "Телефон - обов'язковий");
            }
            else
            {
                Match match = Regex.Match(model.PhoneNumber, @"\+38\ \(\d{3}\)\ \d{3}-\d{4}");
                if (!match.Success)
                {
                    ModelState.AddModelError(string.Empty, "Телефонний номер не вірний");
                }
            }
            if (ModelState.IsValid)
            {
                var result = await _userRepository.UpdateUserAsync(currentUser.Id, model.Email, model.UName, model.ULastName, model.PhoneNumber, profileImage);
                if (!result.Succeeded)
                {
                    result.SetModelErrors(ModelState);
                    return View(model);
                }
                ViewBag.Message = "Збережено";
                var redirectUrl1 = Url.Action("Index", "Profile");
                Response.Headers.Append("Refresh", $"2;url={redirectUrl1}");
                return View("~/Views/Home/Messages.cshtml");
            }
            return View(model);

        }

        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string password, string newpassword, string confirmnewpassword)
        {
            var user = await _userRepository.GetUserAsync(User);
            if (user != null)
            {
                if (string.IsNullOrEmpty(password))
                {
                    ModelState.AddModelError(string.Empty, "Старий пароль невірний");
                }
                if (newpassword != confirmnewpassword)
                {
                    ModelState.AddModelError(string.Empty, "Нови паролі не співпадають");
                }
                if (ModelState.IsValid)
                {
                    IdentityResult result = await _userRepository.ChangePasswordAsync(user, password, confirmnewpassword);
                    if (!result.Succeeded)
                    {
                        result.SetModelErrors(ModelState);
                        return View();
                    }
                    ViewBag.Message = "Збережено";
                    var redirectUrl1 = Url.Action("Index", "Profile");
                    Response.Headers.Append("Refresh", $"2;url={redirectUrl1}");
                    return View("~/Views/Home/Messages.cshtml");
                }

            }
            return View();
        }
    }
}
