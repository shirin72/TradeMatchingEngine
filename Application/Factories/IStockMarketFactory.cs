using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Query;

namespace Application.Factories
{
    public interface IStockMarketFactory
    {
        Task<IStockMarketMatchEngineWithState> GetStockMarket(IOrderQueryRepository orderQueryRep, ITradeQueryRespository tradeQueryRep);
    }
}