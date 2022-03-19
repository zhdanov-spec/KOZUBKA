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
    public class DirectoryTrendingSearchesAPIController:Controller
    {
        private readonly IDirectoryTrendingSearchesRepository _directoryTrendingSearchesRepository;
        public DirectoryTrendingSearchesAPIController(IDirectoryTrendingSearchesRepository directoryTrendingSearchesRepository)
        {
            _directoryTrendingSearchesRepository = directoryTrendingSearchesRepository;
        }
        [HttpGet]
        [Route("searchtrendingsearches")]
        public async Task<IActionResult> SearchTrendingSearches(string service)
        {
            if(!string.IsNullOrEmpty(service))
            {
                var trendingSeraches = await _directoryTrendingSearchesRepository.DirectoryTrendingSearches.Where(d => d.Enabled && d.ServiceSite == service).Take(4).ToListAsync();
                return Ok(trendingSeraches);
            }
            else
            {
                return Ok();
            }
        }
    }
}
