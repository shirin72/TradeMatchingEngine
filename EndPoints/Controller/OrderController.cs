using Application.OrderService.OrderCommandHandlers;
using EndPoints.Model;
using Microsoft.AspNetCore.Mvc;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.UnitOfWork;

namespace EndPoints.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IAddOrderCommandHandlers addOrderCommandHandlers;
        public OrderController(IAddOrderCommandHandlers addOrderCommandHandlers)
        {
            this.addOrderCommandHandlers = addOrderCommandHandlers;
        }

        [HttpGet]
        public async Task<string> Index()
        {
            return "test";
        }

        /// <summary>
        /// ProcessOrder
        /// </summary>
        /// <param name="price"></param>
        /// <param name="amount"></param>
        /// <param name="side"></param>
        /// <param name="expDate"></param>
        /// <param name="isFillAndKill"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<int> ProcessOrder(int price, int amount, Side side, bool isFillAndKill, DateTime expDate)
        {
            expDate = DateTime.Now.AddDays(1);

            var result = await addOrderCommandHandlers.Handle(price, amount, side, expDate, isFillAndKill);

            return result;
        }
    }
}
