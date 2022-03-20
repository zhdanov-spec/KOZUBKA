using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ua.kozubka.context.Models.Directory;
using ua.kozubka.context.Services.Repositories.ModelRepository;
using ua.kozubka.context.Services.Repositories.ModelRepository.Directory;
using ua.kozubka.context.Services.Repositories.Partitial;
using ua.kozubka.context.ViewModels.APIViewModels;
using ua.kozubka.context.ViewModels.GeneralViewModels;
using ua.kozubka.context.ViewModels.RROViewModels.ShopViewModels;

namespace ua.kozubka.kasa.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RROGroupAPIController : Controller
    {
        private readonly IDirectoryRroGoodsRepository _directoryRroGoodsRepository;
        private readonly UserRepository _userRepository;
        private readonly IPartialHelper _partialHelper;
        public RROGroupAPIController(IDirectoryRroGoodsRepository directoryRroGoodsRepository,
            UserRepository userRepository,
            IPartialHelper partialHelper)
        {
            _directoryRroGoodsRepository = directoryRroGoodsRepository;
            _userRepository = userRepository;
            _partialHelper = partialHelper;
        }
        [HttpPost]
        [Route("postgroupstructure")]
        public async Task<JsonResult> PostGroupStructure([FromBody] RroGroup structure)
        {
            var user = await _userRepository.GetUserAsync(User);
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false,
            };
            if (structure != null)
            {
                var structureList = JsonSerializer.Deserialize<List<ItemStructure>>(structure.Struct);
                if (structureList != null)
                {
                    foreach (var structureItem in structureList)
                    {
                        await _directoryRroGoodsRepository.RestructureGoodsGroupByUser(structureItem, user);
                    }
                }

            }
            return Json(model);
        }
        [HttpPost]
        [Route("getrrogroup")]
        public async Task<JsonResult> GetRROGroup([FromBody] RROInfoSearchModel rroInfoSearchModel)
        {
            var user = await _userRepository.GetUserAsync(User);
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = true,
            };
            List<DirectoryRroGoodsGroup> items = new List<DirectoryRroGoodsGroup>();
            DirectoryRroGoodsGroup parrentRroGoodsGroup = null;
            if (rroInfoSearchModel.IdRroGoodsGroup == 0)
            {
                items = await _directoryRroGoodsRepository.DirectoryRroGoodsGroups.Where(d => d.ParrentIdRroGoodsGroup == null && d.Enabled && d.User == user).OrderBy(d => d.SortOrder).ToListAsync();
            }
            else
            {
                parrentRroGoodsGroup = await _directoryRroGoodsRepository.DirectoryRroGoodsGroups.Where(d => d.IdRroGoodsGroup == rroInfoSearchModel.IdRroGoodsGroup && d.Enabled && d.User == user).FirstOrDefaultAsync();
                items = await _directoryRroGoodsRepository.DirectoryRroGoodsGroups.Where(d => d.ParrentIdRroGoodsGroup == rroInfoSearchModel.IdRroGoodsGroup && d.Enabled && d.User == user).OrderBy(d => d.SortOrder).ToListAsync();
            }
            if (items.Any())
            {
                foreach (var item in items)
                {
                    model.HtmlString += await _partialHelper.GetPartialHtmlAsStringAsync<DirectoryRroGoodsGroup>("Partial/DirectoryRroGoodsGroup/List/_GroupBlock", item);
                }
            }
            else
            {
                //model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<DirectoryRroGoodsGroup>("Partial/DirectoryRroGoodsGroup/List/_GroupBlock", parrentRroGoodsGroup);
            }
            if (parrentRroGoodsGroup != null)
            {
                if (items.Any())
                {
                    model.HtmlString += await _partialHelper.GetPartialHtmlAsStringAsync<DirectoryRroGoodsGroup>("Partial/DirectoryRroGoodsGroup/List/_GroupBack", parrentRroGoodsGroup);
                }
                else
                {
                    model.HtmlString += await _partialHelper.GetPartialHtmlAsStringAsync<DirectoryRroGoodsGroup>("Partial/DirectoryRroGoodsGroup/List/_GroupBackFull", parrentRroGoodsGroup);
                }

            }
            return Json(model);
        }
        [HttpPost]
        [Route("getrrogroupmodal")]
        public async Task<JsonResult> GetRROGroupModal([FromBody] RroGroup rroGoodsGroup)
        {
            var user = await _userRepository.GetUserAsync(User);
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false,
            };
            if (rroGoodsGroup == null)
            {
                return Json(model);
            }
            IEnumerable<RroGoodsGroupMenuViewModel> rroGoodsGroupList;
            if (!rroGoodsGroup.IdRroGoodsGroup.HasValue)
            {
                rroGoodsGroupList = await _directoryRroGoodsRepository.GetRroGoodsGroupByUser(await _directoryRroGoodsRepository.GetAllRroGoodsGroupItemByUser(user), null,user);
            }
            else
            {
                bool isBack = bool.Parse(rroGoodsGroup.IsBack);
                if (isBack)
                {
                    rroGoodsGroup.IdRroGoodsGroup = await _directoryRroGoodsRepository.DirectoryRroGoodsGroups.Where(c => c.IdRroGoodsGroup == rroGoodsGroup.IdRroGoodsGroup && c.User==user).Select(d => d.ParrentIdRroGoodsGroup).FirstOrDefaultAsync();
                }
                if (rroGoodsGroup.IdRroGoodsGroup == null)
                {
                    rroGoodsGroupList = await _directoryRroGoodsRepository.GetRroGoodsGroupByUser(await _directoryRroGoodsRepository.GetAllRroGoodsGroupItemByUser(user), null,user);
                }
                else
                {
                    rroGoodsGroupList = await _directoryRroGoodsRepository.GetRroGoodsGroupByUser(await _directoryRroGoodsRepository.GetAllRroGoodsGroupItemByUser(user), rroGoodsGroup.IdRroGoodsGroup,user);
                }

            }
            if (rroGoodsGroupList.Any())
            {
                model.HaveResult = true;
                foreach (var item in rroGoodsGroupList)
                {
                    model.HtmlString += await _partialHelper.GetPartialHtmlAsStringAsync<RroGoodsGroupMenuViewModel>("Partial/DirectoryRroGoodsGroup/Modal/_GroupBlock", item);
                }
                if (rroGoodsGroupList.Any(c => c.ParrentIdRroGoodsGroup != null))
                {
                    model.HtmlString += await _partialHelper.GetPartialHtmlAsStringAsync<RroGoodsGroupMenuViewModel>("Partial/DirectoryRroGoodsGroup/Modal/_GroupBack", rroGoodsGroupList.FirstOrDefault());
                }
            }
            return Json(model);
        }
        public class RroGroup
        {
            public string Struct { get; set; }
            public Int32? IdRroGoodsGroup { get; set; }
            public string IsBack { get; set; }
        }
    }
}
