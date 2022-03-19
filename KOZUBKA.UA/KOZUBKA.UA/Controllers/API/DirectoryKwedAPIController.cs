using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ua.kozubka.context.Services.Repositories.ModelRepository.Directory;
using static ua.kozubka.context.Classes.Constants;

namespace ua.kozubka.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectoryKwedAPIController : Controller
    {
        private readonly IDirectoryKwedRepository _directoryKwedRepository;

        public DirectoryKwedAPIController(IDirectoryKwedRepository directoryKwedRepository)
        {
            _directoryKwedRepository = directoryKwedRepository;

        }
        [HttpGet]
        [Route("searchkwed")]
        public async Task<IActionResult> SearchKwed(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var states = await _directoryKwedRepository.GetSearchKwed(TypeSearch.ByPattern, term);
                return Ok(states);
            }
            else
            {
                return Ok();
            }
        }
    }
}
