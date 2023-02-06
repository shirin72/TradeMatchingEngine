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
        /// 
        /// </summary>
        /// <param name="orderVM"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ProcessedOrder> ProcessOrder([FromBody] OrderVM orderVM)
        {
            var command = new AddOrderCommand()
            {
                Amount = orderVM.Amount,
                ExpDate = orderVM.ExpireTime,
                Side = orderVM.Side,
                Price = orderVM.Price,
                IsFillAndKill = (bool)orderVM.IsFillAndKill,
            };

            return await addOrderCommandHandlers.Handle(command) ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modifieOrderVM"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPut]
        public async Task<long> ModifieOrder([FromBody] ModifiedOrderVM modifieOrderVM)
        {
            var modifieCommand = new ModifieOrderCommand()
            {
                Amount = modifieOrderVM.Amount,
                Price = modifieOrderVM.Price,
                OrderId = modifieOrderVM.OrderId,
                ExpDate = modifieOrderVM.ExpDate,
            };

            var result = await modifieOrderCommandHandler.Handle(modifieCommand);

            if (result != null)
            {
                return result.OrderId;
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
        public async Task<long> CancellOrder([FromBody] CancellOrderVM cancellOrderVM)
        {
            try
            {
                var result = await cancellOrderCommandHandler.Handle(cancellOrderVM.OrderId);

                if (result != null)
                {
                    return (long)result.OrderId;
                }
                throw new Exception("Order Not Found");
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        [HttpGet]
        public async Task<Order> GetOrder(long orderId)
        {
            return await orderQueryRepository.Get(o => o.Id == orderId);
        }
    }
}
