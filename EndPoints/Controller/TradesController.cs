using Domain;
using Domain.Trades.Repositories.Query;
using EndPoints.Model;
using Microsoft.AspNetCore.Mvc;

namespace EndPoints.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradesController : BaseController
    {
        private readonly ITradeQueryRespository queryRepository;

        public TradesController(ITradeQueryRespository queryRepository)
        {
            this.queryRepository = queryRepository;
        }

        [HttpGet(Name = nameof(GetAllTrades))]
        public async Task<IActionResult> GetAllTrades()
        {
            return Ok(await queryRepository.GetAll());
        }

        [HttpGet("{id}", Name = nameof(GetTrade))]
        public async Task<IActionResult> GetTrade(long id)
        {
            return Ok(await queryRepository.Get(t => t.Id == id));
        }
    }
}
