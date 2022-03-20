using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using ua.kozubka.context.Services.Repositories.ModelRepository;
using ua.kozubka.context.Services.Repositories.ModelRepository.Rro;
using ua.kozubka.context.ViewModels.RROViewModels.CheckViewModels;

namespace ua.kozubka.kasa.Controllers
{
    [Authorize]
    
    public class ReportsController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly IRROCheckRepository _rroCheckRepository;
        public ReportsController(UserRepository userRepository,
            IRROCheckRepository rroCheckRepository)
        {
            _userRepository = userRepository;
            _rroCheckRepository = rroCheckRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult CheckDetail(Int32 IdCheck)
        {
            return View(IdCheck); 
        }
    }
}
