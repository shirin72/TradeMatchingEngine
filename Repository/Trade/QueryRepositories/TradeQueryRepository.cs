using Infrastructure.GenericServices;
using Microsoft.EntityFrameworkCore;
using Domain;
using Domain.Trades.Repositories.Query;
namespace Infrastructure.Order.CommandRepositories
{
    public class TradeQueryRepository : QueryRepository<Domain.Trade, Domain.ITrade>,ITradeQueryRespository
    {
        private readonly TradeMatchingEngineContext tradeMatchingEngineContext;

        public TradeQueryRepository(TradeMatchingEngineContext dbcontext) : base(dbcontext)
        {

        }

       
    }
}
