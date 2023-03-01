using Microsoft.AspNetCore.Mvc;
using Domain;
using Domain.Trades.Repositories.Query;

namespace EndPoints.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradesController : ControllerBase
    {
        private readonly ITradeQueryRespository queryRepository;

        public TradesController(ITradeQueryRespository queryRepository)
        {
            this.queryRepository = queryRepository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTrades()
        {
            return Ok(await queryRepository.GetAll());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrade(long id)
        {
            return Ok(await queryRepository.Get(t => t.Id == id));
        }
    }
}
