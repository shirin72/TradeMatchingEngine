﻿using Application.Factories;
using Domain;
using Domain.Orders.Commands;
using Domain.Orders.Repositories.Command;
using Domain.Orders.Repositories.Query;
using Domain.Trades.Repositories.Command;
using Domain.Trades.Repositories.Query;
using Domain.UnitOfWork;

namespace Application.OrderService.OrderCommandHandlers
{
    public class AddOrderCommandHandlers : CommandHandler<AddOrderCommand>, IAddOrderCommandHandlers
    {
        public AddOrderCommandHandlers(IUnitOfWork unitOfWork, IStockMarketFactory stockMarketFactory, IOrderCommandRepository orderCommandRepository, IOrderQueryRepository orderQueryRepository, ITradeCommandRepository tradeCommandRepository, ITradeQueryRespository tradeQueryRespository) : base(unitOfWork, stockMarketFactory, orderCommandRepository, orderQueryRepository, tradeCommandRepository, tradeQueryRespository)
        {
        }
        protected async override Task<ProcessedOrder> SpecificHandle(AddOrderCommand? command)
        {
            var result = await _stockMarketMatchEngine.ProcessOrderAsync(command.Price, command.Amount, command.Side, command.ExpDate, command.IsFillAndKill, command.orderParentId);

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

            var processedOrder = new ProcessedOrder() { OrderId = result.Order == null ? 0 : result.Order.Id, Trades = result.CreatedTrades };

            return processedOrder;
        }


    }
}
