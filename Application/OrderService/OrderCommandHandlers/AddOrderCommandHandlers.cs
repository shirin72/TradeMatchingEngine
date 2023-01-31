using Application.Factories;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Commands;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Query;
using TradeMatchingEngine.UnitOfWork;

namespace Application.OrderService.OrderCommandHandlers
{
    public class AddOrderCommandHandlers : CommandHandler<AddOrderCommand>, IAddOrderCommandHandlers
    {
        public AddOrderCommandHandlers(IUnitOfWork unitOfWork, IStockMarketFactory stockMarketFactory, IOrderCommandRepository orderCommandRepository, IOrderQueryRepository orderQueryRepository, ITradeCommandRepository tradeCommandRepository, ITradeQueryRespository tradeQueryRespository) : base(unitOfWork, stockMarketFactory, orderCommandRepository, orderQueryRepository, tradeCommandRepository, tradeQueryRespository)
        {
        }
        protected async override Task<long> SpecificHandle(AddOrderCommand? command, StockMarketEvents? events = null)
        {
            return await this._stockMarketMatchEngine.ProcessOrderAsync(command.Price, command.Amount, command.Side, command.ExpDate, command.IsFillAndKill, command.orderParentId, events);
        }
    }
}
