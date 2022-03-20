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

namespace ua.kozubka.kasa.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RROSmenaAPIController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly IRROSmenaRepository _rroSmenaRepository;
        private readonly IPartialHelper _partialHelper;
        public RROSmenaAPIController(UserRepository userRepository,
            IRROSmenaRepository rroSmenaRepository,
            IPartialHelper partialHelper)
        {
            _userRepository = userRepository;
            _rroSmenaRepository = rroSmenaRepository;
            _partialHelper = partialHelper;
        }
        [HttpGet]
        [Route("getsmenainfo")]
        public async Task<JsonResult> GetSmenaInfo()
        {
            var user = await _userRepository.GetUserAsync(User);
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false
            };
            var smenaInfo = await _rroSmenaRepository.RROSmenas.Where(d => d.User == user && d.IsActive).FirstAsync();
            model.HaveResult = true;
            model.HtmlString =await _partialHelper.GetPartialHtmlAsStringAsync<RROSmena>("Partial/RROSmena/_SmenaInfo", smenaInfo);
            return Json(model);
        }
    }
}
