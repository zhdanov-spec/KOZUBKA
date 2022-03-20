using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ua.kozubka.context.Models.Directory;
using ua.kozubka.context.Models.RRO;
using ua.kozubka.context.Models.RRO.Services.CheckBox;
using ua.kozubka.context.Models.RRO.Services.CheckBox.Method;
using ua.kozubka.context.Services.Repositories.ModelRepository;
using ua.kozubka.context.Services.Repositories.ModelRepository.Directory;
using ua.kozubka.context.Services.Repositories.ModelRepository.Rro;
using ua.kozubka.context.Services.Repositories.RROSerivicesAPI;
using ua.kozubka.context.ViewModels.RROViewModels.ShopViewModels;

namespace ua.kozubka.kasa.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly IDirectoryRroGoodsRepository _directoryRroGoodsRepository;
        private readonly UserRepository _userRepository;
        private readonly IRROSettingRepository _rroSettingRepository;
        private readonly ICheckBoxRepository _checkBoxRepository;
        public SettingsController(IDirectoryRroGoodsRepository directoryRroGoodsRepository,
            UserRepository userRepository,
            IRROSettingRepository rroSettingRepository,
            ICheckBoxRepository checkBoxRepository)
        {
            _directoryRroGoodsRepository = directoryRroGoodsRepository;
            _userRepository = userRepository;
            _rroSettingRepository = rroSettingRepository;
            _checkBoxRepository = checkBoxRepository;
        }

        #region Goods Group Function
        public async Task<IActionResult> GoodsGroupList()
        {
            var user = await _userRepository.GetUserAsync(User);
            if (user == null) return NotFound();
            var model = await _directoryRroGoodsRepository.GetRroGoodsGroupByUser(await _directoryRroGoodsRepository.GetAllRroGoodsGroupItemByUser(user), null,user);
            return View("~/Views/Settings/Groups/GoodsGroupList.cshtml",model);
        }
        public async Task<IActionResult> GoodsGroupListEdit(int? idRroGoodsGroup)
        {
            var user = await _userRepository.GetUserAsync(User);
            if (user == null) return NotFound();
            DirectoryRroGoodsGroup model= new DirectoryRroGoodsGroup();
            if(idRroGoodsGroup.HasValue)
            {
                model = await _directoryRroGoodsRepository.DirectoryRroGoodsGroups.Where(d => d.IdRroGoodsGroup == idRroGoodsGroup.Value && d.User == user).FirstAsync();
            }
            else
            {
                model.Enabled=true;
            }
            return View("~/Views/Settings/Groups/GoodsGroupListEdit.cshtml", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoodsGroupListEdit(DirectoryRroGoodsGroup model)
        {
            var user = await _userRepository.GetUserAsync(User);
            if (user == null) return NotFound();
            model.User = user;
            await _directoryRroGoodsRepository.AddUpdateRroGroodsGroupByUser(model);
            return RedirectToAction("GoodsGroupList");
        }
        #endregion
        #region Goods Item Function
        public IActionResult GoodsItemList()
        {
            return View("~/Views/Settings/Goods/GoodsItemList.cshtml");
        }
        public async Task<IActionResult> GoodsItemListEdit(int? idRroGoodsItem=null)
        {
            var user = await _userRepository.GetUserAsync(User);
            if (user == null) return NotFound();
            RROItemModel model = new RROItemModel();
            var rroitem = await _directoryRroGoodsRepository.DirectoryRroGoodsItems.Include(d=>d.DirectoryRroGoodsGroup).Include(d=>d.User).Where(d => d.IdRroGoodsItem == idRroGoodsItem && d.User == user).FirstOrDefaultAsync();
            if (rroitem==null)
            {
                model.DirectoryRroGoodsItem = new DirectoryRroGoodsItem();
                model.DirectoryRroGoodsItem.Enabled = true;
            }
            else
            {
                model.DirectoryRroGoodsItem= rroitem;
                model.IdRroGoodsGroup = rroitem.DirectoryRroGoodsGroup.IdRroGoodsGroup;
            }
            return View("~/Views/Settings/Goods/GoodsItemListEdit.cshtml",model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GoodsItemListEdit(RROItemModel model)
        {
            var user = await _userRepository.GetUserAsync(User);
            model.DirectoryRroGoodsItem.User = user;
            //1 Если товар не спризнаком 0 то проверим товар этого пользователя или нет
            DirectoryRroGoodsItem findItem = new DirectoryRroGoodsItem();
            if(model.DirectoryRroGoodsItem.IdRroGoodsItem!=0)
            {
                findItem = await _directoryRroGoodsRepository.DirectoryRroGoodsItems.Include(d => d.DirectoryRroGoodsGroup).Include(d => d.User).Where(d => d.IdRroGoodsItem == model.DirectoryRroGoodsItem.IdRroGoodsItem && d.User == user).FirstAsync();
                if(findItem==null)
                {
                    ModelState.AddModelError(string.Empty, "Редагувати не своїт товари неможна");
                }
            }
            //2 Проверим заполнено ли поле группа товаров
            var findRroGoodsGroup = await _directoryRroGoodsRepository.DirectoryRroGoodsGroups.Where(d=>d.IdRroGoodsGroup==model.IdRroGoodsGroup && d.User == user).FirstOrDefaultAsync();
            if(findRroGoodsGroup==null)
            {
                ModelState.AddModelError(string.Empty, "Не змогли визначити групу товарів");
            }
            else
            {
                model.DirectoryRroGoodsItem.DirectoryRroGoodsGroup = findRroGoodsGroup;
                ModelState.Remove("DirectoryRroGoodsItem.DirectoryRroGoodsGroup");
            }
            if((float)model.DirectoryRroGoodsItem.Price<0.01)
            {
                ModelState.AddModelError(string.Empty, "Ціна не може бути меньш за 0.01");
            }
            if(ModelState.IsValid)
            {
                bool add = await _directoryRroGoodsRepository.AddUpdateRroGroodsItemByUser(model.DirectoryRroGoodsItem,user);
                return RedirectToAction(nameof(GoodsItemList));
            }
            return View("~/Views/Settings/Goods/GoodsItemListEdit.cshtml", model);
        }
        #endregion
        #region Services Function
        public async Task<IActionResult> ServicesList()
        {
            var user = await _userRepository.GetUserAsync(User);
            if (user == null) return NotFound();
            var model = await _rroSettingRepository.RROSettings.Where(d => d.User == user).FirstOrDefaultAsync();
            if(model == null)
            {
                await _userRepository.CheckRROUser(user);
                model = await _rroSettingRepository.RROSettings.Where(d => d.User == user).FirstOrDefaultAsync();
            }
            return View("~/Views/Settings/Services/ChangeService.cshtml", model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServicesList(RROSetting model)
        {
            var user = await _userRepository.GetUserAsync(User);
            if (user == null) return NotFound();
            await _rroSettingRepository.UpdateSetting(user, model);
            switch(model.RROService)
            {
                case context.Classes.Constants.RROService.Checkbox:
                    SignInCashier signInCashier =  await _checkBoxRepository.SignIn(model.CHECKBOX_CASHIER_LOGIN, model.CHECKBOX_CASHIER_PASSWORD);
                    if(signInCashier != null)
                    {
                        await _rroSettingRepository.UpdateCheckBoxToken(user, signInCashier.access_token);
                    }
                    break;
            }
            return RedirectToAction("Index", "Shop");
        }
        #endregion
    }
}
