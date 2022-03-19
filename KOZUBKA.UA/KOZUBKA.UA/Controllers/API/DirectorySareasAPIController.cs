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
    public class DirectorySareasAPIController : Controller
    {
        private readonly IDirectorySareasRepository _directorySareasRepository;
        public DirectorySareasAPIController(IDirectorySareasRepository directorySareasRepository)
        {

            _directorySareasRepository = directorySareasRepository;
        }
        [HttpGet]
        [Route("searchsarea")]
        public async Task<IActionResult> SearchSarea(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                var states = await _directorySareasRepository.GetSearchSarea(TypeSearch.ByPattern, term);
                return Ok(states);
            }
            else
            {
                return Ok();
            }
        }
    }


   
}
