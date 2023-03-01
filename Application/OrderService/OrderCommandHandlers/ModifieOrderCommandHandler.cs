using Application.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Domain.Orders.Commands;
using Domain.Orders.Repositories.Command;
using Domain.Orders.Repositories.Query;
using Domain.Trades.Repositories.Command;
using Domain.Trades.Repositories.Query;
using Domain.UnitOfWork;

namespace Application.OrderService.OrderCommandHandlers
{
    public class ModifieOrderCommandHandler : CommandHandler<ModifieOrderCommand>, IModifieOrderCommandHandler
    {
        public ModifieOrderCommandHandler(IUnitOfWork unitOfWork, IStockMarketFactory stockMarketFactory, IOrderCommandRepository orderCommandRepository, IOrderQueryRepository orderQueryRepository, ITradeCommandRepository tradeCommandRepository, ITradeQueryRespository tradeQueryRespository) : base(unitOfWork, stockMarketFactory, orderCommandRepository, orderQueryRepository, tradeCommandRepository, tradeQueryRespository)
        {
        }

        protected async override Task<ProcessedOrder> SpecificHandle(ModifieOrderCommand? command)
        {
            try
            {
                var result = await this._stockMarketMatchEngine.ModifieOrder(command.OrderId, command.Price, command.Amount, command.ExpDate);

                await _orderCommandRepository.Add(result.Order);

                foreach (var order in result.ModifiedOrders)
                {
                    var findOrder = await this._orderCommandRepository.Find(order.Id);
                    findOrder.UpdateBy(order);
                }

                foreach (var trade in result.CreatedTrades)
                {
                    await _tradeCommandRepository.Add(trade);
                }
                return new ProcessedOrder() { OrderId = result.Order == null ? 0 : result.Order.Id };
            }
            catch (Exception ex)
            {

                throw;
            }

        }
    }
}
