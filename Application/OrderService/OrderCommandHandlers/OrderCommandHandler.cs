using Application.Factories;
using Domain;
using Domain.Orders.Repositories.Command;
using Domain.Orders.Repositories.Query;
using Domain.Trades.Repositories.Command;
using Domain.Trades.Repositories.Query;
using Domain.UnitOfWork;

namespace Application.OrderService.OrderCommandHandlers
{
    public abstract class CommandHandler<T1> : ICommandHandler<T1>
    {
        protected readonly IStockMarketFactory _stockMarketFactory;
        protected readonly IOrderCommandRepository _orderCommandRepository;
        protected readonly IOrderQueryRepository _orderQuery;
        protected readonly ITradeQueryRespository _tradeQuery;
        protected readonly ITradeCommandRepository _tradeCommandRepository;
        protected readonly IUnitOfWork _unitOfWork;
        protected IStockMarketMatchEngineWithState _stockMarketMatchEngine;
        public CommandHandler(IUnitOfWork unitOfWork, IStockMarketFactory stockMarketFactory, IOrderCommandRepository orderCommandRepository, IOrderQueryRepository orderQueryRepository, ITradeCommandRepository tradeCommandRepository, ITradeQueryRespository tradeQueryRespository)
        {
            _stockMarketFactory = stockMarketFactory;
            _orderCommandRepository = orderCommandRepository;
            _orderQuery = orderQueryRepository;
            _tradeCommandRepository = tradeCommandRepository;
            _tradeQuery = tradeQueryRespository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ProcessedOrder?> Handle(T1 command)
        {
            _stockMarketMatchEngine = await _stockMarketFactory.GetStockMarket(_orderQuery, _tradeQuery);

            var result = await SpecificHandle(command);

            await _unitOfWork.SaveChange();

            return result;
        }

        protected abstract Task<ProcessedOrder> SpecificHandle(T1? command);

    }
}