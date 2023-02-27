using Microsoft.AspNetCore.Mvc;
using TradeMatchingEngine;
using TradeMatchingEngine.Trades.Repositories.Query;

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
        public async Task<IEnumerable<ITrade>> GetAllTrades()
        {
            return await queryRepository.GetAll();
        }
        [HttpGet("{id}")]
        public async Task<ITrade> GetTrade(long id)
        {
            return await queryRepository.Get(t=>t.Id == id);
        }
    }
}
