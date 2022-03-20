using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ua.kozubka.context.Models.RRO;
using ua.kozubka.context.Models.RRO.Services.CheckBox.Method;
using ua.kozubka.context.Services.Repositories.ModelRepository;
using ua.kozubka.context.Services.Repositories.ModelRepository.Directory;
using ua.kozubka.context.Services.Repositories.ModelRepository.Rro;
using ua.kozubka.context.Services.Repositories.Partitial;
using ua.kozubka.context.Services.Repositories.RROSerivicesAPI;
using ua.kozubka.context.ViewModels.APIViewModels;
using ua.kozubka.context.ViewModels.RROViewModels.CheckViewModels;
using ua.kozubka.context.ViewModels.RROViewModels.ShopViewModels;

namespace ua.kozubka.kasa.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RROCartAPIController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly RROCart _rroCart;
        private readonly IDirectoryRroGoodsRepository _directoryRroGoodsRepository;
        private readonly IPartialHelper _partialHelper;
        private readonly IRROCheckRepository _rroCheckRepository;
        private readonly IRROSettingRepository _rroSettingRepository;
        private readonly ICheckBoxRepository _checkBoxRepository;
        public RROCartAPIController(UserRepository userRepository,
            RROCart rroCart,
            IDirectoryRroGoodsRepository directoryRroGoodsRepository,
            IPartialHelper partialHelper,
            IRROCheckRepository rroCheckRepository,
            IRROSettingRepository rroSettingRepository,
            ICheckBoxRepository checkBoxRepository)
        {
            _userRepository = userRepository;
            _rroCart = rroCart;
            _directoryRroGoodsRepository = directoryRroGoodsRepository;
            _partialHelper = partialHelper;
            _rroCheckRepository = rroCheckRepository;
            _rroSettingRepository = rroSettingRepository;
            _checkBoxRepository = checkBoxRepository;
        }
        [HttpPost]
        [Route("addcart")]
        public async Task<JsonResult> AddCart([FromBody] RROInfoSearchModel rroInfoSearchModel)
        {
            var user = await _userRepository.GetUserAsync(User);
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false
            };
            var rroItem = await _directoryRroGoodsRepository.DirectoryRroGoodsItems.Where(d => d.Enabled && d.User == user && d.IdRroGoodsItem==rroInfoSearchModel.IdRroGoodsItem).FirstOrDefaultAsync();
            if (rroItem == null)
            {
                model.HtmlErrorText = "Неможливо визначити продукт";
                return Json(model);
            }
            bool result = await _rroCart.AddToRROCart(rroItem, 1, user);
            if(!result)
            {
                model.HtmlErrorText = "Помилка додавання";
                return Json(model);
            }
            var rroCartPositions = await _rroCart.GetRROCartPositions(user);
            model.HaveResult = true;
            model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<IEnumerable<RROCartViewModel>>("Partial/RROCart/_IconCart", rroCartPositions);
            return Json(model);
        }
        [HttpPost]
        [Route("updatecount")]
        public async Task<JsonResult> UpdateCount([FromBody] RROInfoSearchModel rroInfoSearchModel)
        {
            var user = await _userRepository.GetUserAsync(User);
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false
            };
            var rroItem = await _directoryRroGoodsRepository.DirectoryRroGoodsItems.Where(d => d.Enabled && d.User == user && d.IdRroGoodsItem == rroInfoSearchModel.IdRroGoodsItem).FirstOrDefaultAsync();
            if (rroItem == null)
            {
                model.HtmlErrorText = "Неможливо визначити продукт";
                return Json(model);
            }
            bool result = await _rroCart.ChangeCountInRROCart(rroItem, rroInfoSearchModel.Count, user);
            if(result)
            {
                var rroCartPositions = await _rroCart.GetRROCartPositions(user);
                var rroCartPosition = rroCartPositions.Where(d => d.DirectoryRroGoodsItem.IdRroGoodsItem == rroInfoSearchModel.IdRroGoodsItem).First();
                model.HaveResult = true;
                model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<RROCartViewModel>("Partial/RROCart/_CartItem", rroCartPosition);
                model.HtmlCount = rroCartPositions.Sum(d => d.RROCartPosition.Summa).ToString();
            }
            return Json(model);
        }

        [HttpPost]
        [Route("removecart")]
        public async Task<JsonResult> RemoveCart([FromBody] RROInfoSearchModel rroInfoSearchModel)
        {
            var user = await _userRepository.GetUserAsync(User);
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false
            };
            var rroItem = await _directoryRroGoodsRepository.DirectoryRroGoodsItems.Where(d => d.Enabled && d.User == user && d.IdRroGoodsItem == rroInfoSearchModel.IdRroGoodsItem).FirstOrDefaultAsync();
            if (rroItem == null)
            {
                model.HtmlErrorText = "Неможливо визначити продукт";
                return Json(model);
            }
            bool result = await _rroCart.RemoveFromRROCart(rroItem, user);
            if (result)
            {
                var rroCartPositions = await _rroCart.GetRROCartPositions(user);
                model.HaveResult = true;
                model.HtmlString = rroCartPositions.Sum(d => d.RROCartPosition.Summa).ToString();
            }
            return Json(model);
        }
        [HttpPost]
        [Route("createreceipt")]
        public async Task<JsonResult> CreateReceipt([FromBody] RROInfoSearchModel rroInfoSearchModel)
        {
            context.Classes.Constants.PaydType paydType = context.Classes.Constants.PaydType.Cash;
            
            var user = await _userRepository.GetUserAsync(User);
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false
            };
            try
            {
                paydType = (context.Classes.Constants.PaydType)rroInfoSearchModel.PaydType;
            }
            catch
            {
                return Json(model);
            }
            RROCheckViewModel rroCheck = await _rroCheckRepository.CreateRROCheck(user, paydType);
            await _rroCart.ClearRROCart(user);

            if(rroCheck==null)
            {
                return Json(model);
            }
            RROSetting rroSetting =await  _rroSettingRepository.RROSettings.Where(d => d.User == user).FirstAsync();
            
            if(rroSetting!=null)
            {
                RROReturnedViewModel rroModel = new RROReturnedViewModel();
                
                switch (rroSetting.RROService)
                {
                    case context.Classes.Constants.RROService.None:
                        model.HaveResult = true;
                        model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<RROCheckViewModel>("Partial/RROCheck/Reports/Detail/_ServiceNone", rroCheck);
                        break;
                    case context.Classes.Constants.RROService.Checkbox:
                        rroModel=await _checkBoxRepository.CreateCheck(rroCheck, rroSetting);
                        if(rroModel.StatusCode==System.Net.HttpStatusCode.Created)
                        {
                            model.HaveResult = true;
                            CreateReceipt createReceipt = JsonConvert.DeserializeObject<CreateReceipt>(rroModel.Data);
                            await _rroCheckRepository.SetIdentityRROCheck(user, rroCheck.RROCheck.IdRROCheck, createReceipt.id, rroSetting.RROService);
                            model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<RROCheckViewModel>("Partial/RROCheck/Reports/Detail/_ServiceCheckBox", rroCheck);
                        }
                        break;
                    default:
                        model.HaveResult = true;
                        model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<RROCheckViewModel>("Partial/RROCheck/Reports/Detail/_ServiceNone", rroCheck);
                        break;
                }

            }

            return Json(model);
        }
        [HttpPost]
        [Route("updatediscount")]
        public async Task<JsonResult> UpdateDiscount([FromBody] RROInfoSearchModel rroInfoSearchModel)
        {
            var user = await _userRepository.GetUserAsync(User);
            ReturnedViewModel model = new ReturnedViewModel
            {
                HaveResult = false
            };
            var rroItem = await _directoryRroGoodsRepository.DirectoryRroGoodsItems.Where(d => d.Enabled && d.User == user && d.IdRroGoodsItem == rroInfoSearchModel.IdRroGoodsItem).FirstOrDefaultAsync();
            if (rroItem == null)
            {
                model.HtmlErrorText = "Неможливо визначити продукт";
                return Json(model);
            }
            bool result = await _rroCart.ChangeDiscountInRROCart(rroItem, rroInfoSearchModel.DiscountSum, user);
            if (result)
            {
                var rroCartPositions = await _rroCart.GetRROCartPositions(user);
                var rroCartPosition = rroCartPositions.Where(d => d.DirectoryRroGoodsItem.IdRroGoodsItem == rroInfoSearchModel.IdRroGoodsItem).FirstOrDefault();
                if (rroCartPosition != null)
                {
                    model.HaveResult = true;
                    model.HtmlString = await _partialHelper.GetPartialHtmlAsStringAsync<RROCartViewModel>("Partial/RROCart/_CartItem", rroCartPosition);
                    model.HtmlCount = rroCartPositions.Sum(d => d.RROCartPosition.Summa - d.RROCartPosition.DiscountSum).ToString();
                }
            }
            return Json(model);
        }
    }
}
