using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ua.kozubka.context.Services.Repositories.ModelRepository;

namespace ua.kozubka.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImagesAPIController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly ISettingRepository _settingRepository;
        public ImagesAPIController(UserRepository userRepository,
            ISettingRepository settingRepository)
        {
            _userRepository = userRepository;
            _settingRepository = settingRepository;
        }
        [HttpGet]
        [Route("getfile")]
        public async Task<IActionResult> GetFile([FromQuery] string id)
        {
            string fileSite = _settingRepository.Settings.Where(d => d.Key == ua.kozubka.context.Classes.Constants.SettingsOptions.AbsolutePathImage.ToString()).Select(d => d.Value).SingleOrDefault();
            id = id.Replace("/", "\\");
            string file = string.Format("{0}{1}", fileSite, id);

            var readFile = await System.IO.File.ReadAllBytesAsync(file);
            if (readFile == null)
                return NotFound();
            return File(readFile, "image/*");
        }
        [HttpGet]
        [Route("deleteprofileimage")]
        public async Task<IActionResult> DeleteProfileImage([FromQuery] string id)
        {
            var user = await _userRepository.GetUserAsync(User);
            if(user!=null)
            {
                await _userRepository.DeteleteProfileImage(id, user);
            }
            
            return Ok();
        }
    }
}
