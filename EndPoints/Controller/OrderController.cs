using Application.OrderService.OrderCommandHandlers;
using Microsoft.AspNetCore.Mvc;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Commands;

namespace EndPoints.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

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
        public async Task<long> ProcessOrder([FromServices] IAddOrderCommandHandlers handler, int price, int amount, Side side, bool isFillAndKill, DateTime? expDate)
        {
            var command = new AddOrderCommand()
            {
                Amount = amount,
                ExpDate = expDate,
                Side = side,
                Price = price,
                IsFillAndKill = isFillAndKill,
            };

            return await handler.Handle(command) ?? 0;
        }

        /// <summary>
        /// ModifieOrder
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="orderId"></param>
        /// <param name="price"></param>
        /// <param name="amount"></param>
        /// <param name="expDate"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<long> ModifieOrder([FromServices] IModifieOrderCommandHandler handler, long orderId, int price, int amount, DateTime? expDate)
        {
            var modifieCommand = new ModifieOrderCommand()
            {
                Amount = amount,
                Price = price,
                OrderId = orderId,
                ExpDate = expDate,
            };

            var result = await handler.Handle(modifieCommand);

            if (result != null)
            {
                return (long)result;
            }

            throw new Exception("Order Not Found");
        }

        /// <summary>
        /// CancellOrder
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPatch]
        public async Task<long> CancellOrder([FromServices] ICancellOrderCommandHandler handler, long orderId)
        {
            var result = await handler.Handle(orderId);

            if (result != null)
            {
                return (long)result;
            }
            throw new Exception("Order Not Found");
        }
    }
}
