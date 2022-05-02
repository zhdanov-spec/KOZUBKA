using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using ua.kozubka.context.Services.Repositories.Messangers.Telegram.Teams;

namespace ua.kozubka.where.Controllers.API
{
    [Route("api/[controller]/update")]
    [ApiController]
    public class TelegramAPIController : Controller
    {
        private ITelegramBotWhereTeamRepository _telegramBotWhereTeamRepository;
        public TelegramAPIController(ITelegramBotWhereTeamRepository telegramBotWhereTeamRepository)
        {
            _telegramBotWhereTeamRepository = telegramBotWhereTeamRepository;
        }
        [HttpGet]
        public string Get()
        {

            return "Method GET unuvalable";
        }
        [HttpPost]

        public async Task<IActionResult> Post([FromBody] Update update)
        {
            await _telegramBotWhereTeamRepository.EchoAsync(update);
            return Ok();
        }
    }
}
