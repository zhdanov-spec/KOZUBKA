using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ua.kozubka.context.Models.RRO;
using ua.kozubka.context.Services.Repositories.ModelRepository;
using ua.kozubka.context.Services.Repositories.ModelRepository.Rro;
using ua.kozubka.context.Services.Repositories.Partitial;
using ua.kozubka.context.ViewModels.APIViewModels;
using ua.kozubka.context.ViewModels.RROViewModels.CheckViewModels;
using ua.kozubka.context.ViewModels.RROViewModels.ShopViewModels;

namespace ua.kozubka.kasa.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RROReportsAPIController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly IRROCheckRepository _rroCheckRepository;
        private readonly IPartialHelper _partialHelper;
        public RROReportsAPIController(UserRepository userRepository,
            IRROCheckRepository rroCheckRepository,
            IPartialHelper partialHelper)
        {
            _userRepository = userRepository;
            _rroCheckRepository = rroCheckRepository;
            _partialHelper = partialHelper;
        }
        [HttpPost]
        [Route("getchecksbydate")]
        public async Task<JsonResult> GetChecksByDate([FromBody] RROInfoSearchModel rroInfoSearchModel)
        {
            var user = await _userRepository.GetUserAsync(User);
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false
            };
            if (user == null) return Json(model);
            var rroCheck = await _rroCheckRepository.RROChecks.Where(d=>d.User==user && d.CheckTime.Date==rroInfoSearchModel.UTCDate.Date.Date).OrderByDescending(d=>d.CheckTime).ToListAsync();
            if(rroCheck.Any())
            {
                model.HaveResult = true;
                foreach (var item in rroCheck)
                {
                    model.HtmlString += await _partialHelper.GetPartialHtmlAsStringAsync<RROCheck>("Partial/RROCheck/Reports/_RROCheckItem", item);
                }

            }
            else
            {
                model.HaveResult = true;
                model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<RROCheck>("Partial/RROCheck/Reports/_RROCheckNoItem",null);
            }
            
            return Json(model);
        }
        [HttpPost]
        [Route("getcheck")]
        public async Task<JsonResult> GetCheck([FromBody] RROInfoSearchModel rroInfoSearchModel)
        {
            var user = await _userRepository.GetUserAsync(User);
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false
            };
            if (user == null) return Json(model);
            var check = await _rroCheckRepository.CheckDetail(user, rroInfoSearchModel.IdCheck);
            if(check!=null)
            {
                model.HaveResult = true;
                switch (check.RROCheck.RROService)
                {
                    case context.Classes.Constants.RROService.None:
                        model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<RROCheckViewModel>("Partial/RROCheck/Reports/Detail/_ServiceNone", check);
                        break;
                    case context.Classes.Constants.RROService.Checkbox:
                        model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<RROCheckViewModel>("Partial/RROCheck/Reports/Detail/_ServiceCheckBox", check);
                        break;
                    default:
                        model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<RROCheckViewModel>("Partial/RROCheck/Reports/Detail/_ServiceNone", check);
                        break;
                }
            }
            return Json(model);
        }
    }
}
