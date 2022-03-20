using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ua.kozubka.context.Services.Repositories.ModelRepository;
using ua.kozubka.context.Services.Repositories.ModelRepository.Rro;
using ua.kozubka.context.ViewModels.RROViewModels.CheckViewModels;
using ua.kozubka.context.ViewModels.RROViewModels.ShopViewModels;

namespace ua.kozubka.kasa.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly RROCart _rroCart;
       
        public ShopController(UserRepository userRepository,
            RROCart rroCart
            )
        {
            _userRepository= userRepository;
            _rroCart = rroCart;
           
        }
        public IActionResult Index(string keywordSearch,Int32? IdRroGoodsGroup)
        {
            RROInfoSearchModel model = new RROInfoSearchModel
            {
                Search = keywordSearch,
                IdRroGoodsGroup = IdRroGoodsGroup
            };
            return View(model);
        }
        public async Task<IActionResult> Receipt()
        {
            var user = await _userRepository.GetUserAsync(User);
            if (user == null) return NotFound();
            var model = await _rroCart.GetRROCartPositions(user);
            return View(model);
        }
        
    }
}
