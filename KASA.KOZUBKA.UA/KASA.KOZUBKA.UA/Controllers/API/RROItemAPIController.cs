using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ua.kozubka.context.Services.Repositories.ModelRepository;
using ua.kozubka.context.Services.Repositories.ModelRepository.Directory;
using ua.kozubka.context.Services.Repositories.Partitial;
using ua.kozubka.context.ViewModels.APIViewModels;
using ua.kozubka.context.ViewModels.RROViewModels.ShopViewModels;

namespace ua.kozubka.kasa.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RROItemAPIController : Controller
    {
        private readonly IDirectoryRroGoodsRepository _directoryRroGoodsRepository;
        private readonly UserRepository _userRepository;
        private readonly IPartialHelper _partialHelper;
        public RROItemAPIController(IDirectoryRroGoodsRepository directoryRroGoodsRepository,
              UserRepository userRepository,
              IPartialHelper partialHelper)
        {
            _directoryRroGoodsRepository = directoryRroGoodsRepository;
            _userRepository = userRepository;
            _partialHelper = partialHelper;
        }
        [HttpPost]
        [Route("getrroitem")]
        public async Task<JsonResult> GetRROItem([FromBody] RROInfoSearchModel rroInfoSearchModel)
        {
            var user =await  _userRepository.GetUserAsync(User);
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = true,
            };
            var items = (from rroItem in _directoryRroGoodsRepository.DirectoryRroGoodsItems
                         .Include(d=>d.DirectoryRroGoodsGroup)
                         .Include(d=>d.User)
                         .Where(d=>d.User==user)
                         .OrderByDescending(d => d.UpdateTime)
                         select new RROItemModel
                         {
                              DirectoryRroGoodsItem = rroItem,
                              IsEdit=rroInfoSearchModel.IsEdit
                              
                         }
                         ).AsNoTracking().AsExpandable();
            if(rroInfoSearchModel.IdRroGoodsGroup.HasValue)
            {
                if(rroInfoSearchModel.IdRroGoodsGroup!=0)
                {
                    IList<int> goodsCategoryWithParrent = await _directoryRroGoodsRepository.GetMenuChildRroGoodsGroupByUser(await _directoryRroGoodsRepository.GetAllRroGoodsGroupItemByUser(user), rroInfoSearchModel.IdRroGoodsGroup.Value,user);
                    items = items.Where(d => goodsCategoryWithParrent.Contains(d.DirectoryRroGoodsItem.DirectoryRroGoodsGroup.IdRroGoodsGroup) || d.DirectoryRroGoodsItem.DirectoryRroGoodsGroup.IdRroGoodsGroup == rroInfoSearchModel.IdRroGoodsGroup.Value);
                }
            }
            if (items.Any())
            {
                foreach (var item in items)
                {
                    model.HtmlString += await _partialHelper.GetPartialHtmlAsStringAsync<RROItemModel>("Partial/DirectoryRroGoodsItmes/List/_ItemBlock", item);
                }
            }
            else
            {
                if (rroInfoSearchModel.IdRroGoodsGroup == 0)
                {
                    model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<RROItemModel>("Partial/DirectoryRroGoodsItmes/List/_NoItem", null);
                }
                else
                {
                    model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<RROItemModel>("Partial/DirectoryRroGoodsItmes/List/_ItemEmpty", null);
                }
            }
            return Json(model);
        }
    }
}
