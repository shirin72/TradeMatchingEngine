using Domain;
using Domain.Orders.Repositories.Query;
using Domain.Trades.Repositories.Query;

namespace Application.Factories
{
    public interface IStockMarketFactory
    {
        Task<IStockMarketMatchEngineWithState> GetStockMarket(IOrderQueryRepository orderQueryRep, ITradeQueryRespository tradeQueryRep);
    }
}