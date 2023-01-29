using Application.OrderService.OrderCommandHandlers;
using Microsoft.AspNetCore.Mvc;
using TradeMatchingEngine;

namespace EndPoints.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        public OrderController()
        {

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
        [HttpPatch]
        public async Task<long> ProcessOrder([FromServices] IAddOrderCommandHandlers handler, int price, int amount, Side side, bool isFillAndKill, DateTime? expDate)
        {
            try
            {
                return await handler.Handle(price, amount, side, expDate, isFillAndKill);
            }
            catch (Exception ex)
            {
                throw;
            }
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
            try
            {
                var result = await handler.Handle(orderId, price, amount, expDate);

                if (result != null)
                {
                    return (long)result;
                }
                throw new Exception("Order Not Found");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// CancellOrder
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<long> CancellOrder([FromServices] ICancellOrderCommandHandler handler, long orderId)
        {
            try
            {
                var result = await handler.Handle(orderId);

                if (result != null)
                {
                    return (long)result;
                }
                throw new Exception("Order Not Found");
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
