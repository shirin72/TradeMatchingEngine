using Microsoft.AspNetCore.Mvc;
using TradeMatchingEngine;
using TradeMatchingEngine.Trades.Repositories.Query;

namespace EndPoints.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TradesController : ControllerBase
    {
        private readonly ITradeQueryRespository queryRepository;

        public TradesController(ITradeQueryRespository queryRepository)
        {
            this.queryRepository = queryRepository;
        }
        [HttpGet]
        public async Task<IEnumerable<Trade>> GetAllTrades()
        {
            return await queryRepository.GetAll();

        }
        [HttpGet]
        public async Task<Trade> GetTrade(long id)
        {
            return await queryRepository.Get(t=>t.Id == id);
        }
    }
}
