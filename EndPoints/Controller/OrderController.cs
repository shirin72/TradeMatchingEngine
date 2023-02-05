using Application.OrderService.OrderCommandHandlers;
using EndPoints.Model;
using Microsoft.AspNetCore.Mvc;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Commands;
using TradeMatchingEngine.Orders.Repositories.Query;

namespace EndPoints.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IAddOrderCommandHandlers addOrderCommandHandlers;
        private readonly IModifieOrderCommandHandler modifieOrderCommandHandler;
        private readonly ICancellOrderCommandHandler cancellOrderCommandHandler;
        private readonly IOrderQueryRepository orderQueryRepository;

        public OrderController(IAddOrderCommandHandlers addOrderCommandHandlers,
            IModifieOrderCommandHandler modifieOrderCommandHandler,
            ICancellOrderCommandHandler cancellOrderCommandHandler,
            IOrderQueryRepository orderQueryRepository)
        {
            this.addOrderCommandHandlers = addOrderCommandHandlers;
            this.modifieOrderCommandHandler = modifieOrderCommandHandler;
            this.cancellOrderCommandHandler = cancellOrderCommandHandler;
            this.orderQueryRepository = orderQueryRepository;
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
        public async Task<long> ProcessOrder([FromBody]OrderVM orderVM)
        {
            var command = new AddOrderCommand()
            {
                Amount = orderVM.Amount,
                ExpDate = orderVM.ExpireTime,
                Side = orderVM.Side,
                Price = orderVM.Price,
                IsFillAndKill = (bool)orderVM.IsFillAndKill,
            };

            return await addOrderCommandHandlers.Handle(command) ?? 0;
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
        public async Task<long> ModifieOrder(long orderId, int price, int amount, DateTime? expDate)
        {
            var modifieCommand = new ModifieOrderCommand()
            {
                Amount = amount,
                Price = price,
                OrderId = orderId,
                ExpDate = expDate,
            };

            var result = await modifieOrderCommandHandler.Handle(modifieCommand);

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
        public async Task<long> CancellOrder(long orderId)
        {
            var result = await cancellOrderCommandHandler.Handle(orderId);

            if (result != null)
            {
                return (long)result;
            }
            throw new Exception("Order Not Found");
        }

        [HttpGet]
        public async Task<Order> GetOrder(long orderId)
        {
            return await orderQueryRepository.Get(o => o.Id == orderId);
        }
    }
}
