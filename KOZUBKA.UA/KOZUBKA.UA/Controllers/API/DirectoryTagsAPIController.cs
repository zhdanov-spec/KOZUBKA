using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ua.kozubka.context.Services.Repositories.ModelRepository.Directory;
using static ua.kozubka.context.Classes.Constants;

namespace ua.kozubka.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectoryTagsAPIController : Controller
    {
        private readonly IDirectoryTagsRepository _directoryTagsRepository;
        public DirectoryTagsAPIController(IDirectoryTagsRepository directoryTagsRepository)
        {
            _directoryTagsRepository = directoryTagsRepository;
        }
        [HttpGet]
        [Route("searchtag")]
        public async Task<IActionResult> SearchTag(string term)
        {
            if(!string.IsNullOrEmpty(term))
            {
                var tags = await _directoryTagsRepository.GetSearchTag(TypeSearch.ByPattern,SiteConstants.work, term);
                return Ok(tags);
            }
            else
            {
                return Ok();
            }
        }
    }
}
