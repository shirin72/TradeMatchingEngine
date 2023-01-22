using TradeMatchingEngine;

namespace Application.OrderService.OrderCommandHandlers
{
    public interface IAddOrderCommandHandlers
    {
        Task<int> Handle(int price, int amount, Side side, DateTime expDate, bool isFillAndKill);
    }
}