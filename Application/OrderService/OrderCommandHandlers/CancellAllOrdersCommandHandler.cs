using Application.Factories;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Query;
using TradeMatchingEngine.UnitOfWork;

namespace Application.OrderService.OrderCommandHandlers
{

    public class CancellAllOrdersCommandHandler : CommandHandler<object>, ICancellAllOrdersCommandHandler
    {
        public CancellAllOrdersCommandHandler(IUnitOfWork unitOfWork, IStockMarketFactory stockMarketFactory, IOrderCommandRepository orderCommandRepository, IOrderQueryRepository orderQueryRepository, ITradeCommandRepository tradeCommandRepository, ITradeQueryRespository tradeQueryRespository) : base(unitOfWork, stockMarketFactory, orderCommandRepository, orderQueryRepository, tradeCommandRepository, tradeQueryRespository)
        {
        }

        protected async override Task<ProcessedOrder> SpecificHandle(object? obj)
        {
            var allOrders = await _orderQuery.GetAll(x => x.Amount != 0 && x.OrderState != OrderStates.Cancell);

            IStockMarketMatchingEngineProcessContext processedOrder;

            var orderIdList = new List<long>();

            foreach (var item in allOrders)
            {
                processedOrder = await this._stockMarketMatchEngine.CancelOrderAsync(item.Id);

                foreach (var order in processedOrder.ModifiedOrders)
                {
                    var findOrder = await this._orderCommandRepository.Find(order.Id);
                    findOrder.UpdateBy(order);
                    orderIdList.Add(order.Id);
                }
            }

            return new ProcessedOrder() { CancelledOrders = orderIdList };
        }
    }
}
