using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ua.kozubka.context.Services.Repositories.ModelRepository.Directory;

namespace ua.kozubka.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectoryGoodsGroupAPIController : Controller
    {
        
        private readonly IDirectoryGoodsGroupRepository _directoryGoodsGroupRepository;
        public DirectoryGoodsGroupAPIController(IDirectoryGoodsGroupRepository directoryGoodsGroupRepository)
        {
            _directoryGoodsGroupRepository = directoryGoodsGroupRepository;
        }
        [HttpGet]
        [Route("getgroupbyid")]
        public async Task<IActionResult> GetGroupById(int idgroup)
        {
            var group = await _directoryGoodsGroupRepository.DirectoryGoodsGroups.Where(d => d.IdGoodsGroup == idgroup).FirstOrDefaultAsync();
            return Ok(group);
            
        }

    }
}
