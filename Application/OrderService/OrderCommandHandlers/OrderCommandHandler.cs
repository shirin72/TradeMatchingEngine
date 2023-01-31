using Application.Factories;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Query;
using TradeMatchingEngine.UnitOfWork;

namespace Application.OrderService.OrderCommandHandlers
{
    public abstract class CommandHandler<T>:ICommandHandler<T>
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
        public async Task<long?> Handle(T command)
        {
            _stockMarketMatchEngine = await _stockMarketFactory.GetStockMarket(_orderQuery, _tradeQuery);

            var events = new StockMarketEvents()
            {
                OnOrderCreated = async (_stockMarketMatchEngine, order) =>
                {
                    await _orderCommandRepository.Add(order);
                },
                OnTradeCreated = async (_stockMarketMatchEngine, trade) =>
                {
                    await _tradeCommandRepository.Add(trade);
                },
                OnOrderModified = async (_stockMarketMatchEngine, order) =>
                {
                    (await _orderCommandRepository.Find(order.Id))?.UpdateBy(order);
                }
            };

            var result = await SpecificHandle(command, events);

            await _unitOfWork.SaveChange();

            return result;
        }

        protected abstract Task<long> SpecificHandle(T? command, StockMarketEvents? events = null);

    }
}