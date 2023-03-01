using Application.OrderService.OrderCommandHandlers;
using EndPoints.Model;
using Microsoft.AspNetCore.Mvc;
using Domain;
using Domain.Orders.Commands;
using Domain.Orders.Repositories.Query;

namespace EndPoints.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IAddOrderCommandHandlers addOrderCommandHandlers;
        private readonly IModifieOrderCommandHandler modifieOrderCommandHandler;
        private readonly ICancellOrderCommandHandler cancellOrderCommandHandler;
        private readonly IOrderQueryRepository orderQueryRepository;
        private readonly ICancellAllOrdersCommandHandler cancellAllOrderCommandHandler;

        public OrdersController(IAddOrderCommandHandlers addOrderCommandHandlers,
            IModifieOrderCommandHandler modifieOrderCommandHandler,
            ICancellOrderCommandHandler cancellOrderCommandHandler,
            IOrderQueryRepository orderQueryRepository,
                 ICancellAllOrdersCommandHandler cancellAllOrderCommandHandler
            )
        {
            this.addOrderCommandHandlers = addOrderCommandHandlers;
            this.modifieOrderCommandHandler = modifieOrderCommandHandler;
            this.cancellOrderCommandHandler = cancellOrderCommandHandler;
            this.orderQueryRepository = orderQueryRepository;
            this.cancellAllOrderCommandHandler = cancellAllOrderCommandHandler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderVM"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ProcessOrder([FromBody] OrderVM orderVM)
        {
            var command = new AddOrderCommand()
            {
                Amount = orderVM.Amount,
                ExpDate = orderVM.ExpireTime,
                Side = orderVM.Side,
                Price = orderVM.Price,
                IsFillAndKill = (bool)orderVM.IsFillAndKill,
            };

            return CreatedAtAction(
               "ProcessOrder",
                "Orders",
                null,
                await addOrderCommandHandlers.Handle(command));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modifieOrderVM"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPut]
        public async Task<IActionResult> ModifyOrder([FromBody] ModifiedOrderVM modifieOrderVM)
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
                return AcceptedAtAction("ModifyOrder",
                              "Orders",
                null, result.OrderId);
            }

            return BadRequest(modifieOrderVM);
        }

        /// <summary>
        /// CancellOrder
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> CancellOrder(long orderId)
        {
            try
            {
                var result = await cancellOrderCommandHandler.Handle(orderId);

                if (result != null)
                {
                    return AcceptedAtAction(
                                           "CancellOrder",
                                         "Orders",
                                            null, result.OrderId);

                }

                return BadRequest(orderId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// CancellAllOrders
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> CancellAllOrders()
        {
            var result = await cancellAllOrderCommandHandler.Handle(null);

            if (result != null)
            {
                return AcceptedAtAction(
                    "CancellAllOrders",
                      "Orders",
                        null, result.CancelledOrders);
            }

            return BadRequest();
        }


        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(long orderId)
        {
            return Ok(await orderQueryRepository.Get(o => o.Id == orderId));
        }
    }
}
