using TradeMatchingEngine;

namespace Application.OrderService.OrderCommandHandlers
{
    public interface IAddOrderCommandHandlers
    {
        void OnOrderCreated(object sender, EventArgs eventArgs);
        void OnTradeCompleted(object sender, EventArgs eventArgs);
        Task<int> Handle(int price, int amount, Side side, DateTime expDate, bool isFillAndKill);
    }
}