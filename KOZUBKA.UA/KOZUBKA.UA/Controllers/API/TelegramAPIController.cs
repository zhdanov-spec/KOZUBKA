using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using ua.kozubka.context.Services.Repositories.Messangers.Telegram;

namespace ua.kozubka.Controllers.API
{
    [Route("api/[controller]/update")]
    [ApiController]
    public class TelegramAPIController : Controller
    {
        private ITelegramBotRepository _telegramBotRepository;
        public TelegramAPIController(ITelegramBotRepository telegramBotRepository)
        {
            _telegramBotRepository = telegramBotRepository;
        }
        [HttpGet]
        public string Get()
        {
            return "Method GET unuvalable";
        }
        [HttpPost]
        public async Task<OkResult> Post([FromBody] Update update)
        {
            if (update == null) return Ok();
            await _telegramBotRepository.EchoTelegramAsync(update);
            return Ok();
        }
    }
}
