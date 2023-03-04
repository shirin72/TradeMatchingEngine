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
        private readonly IUrlHelper urlHelper;
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
        [HttpPost(Name = nameof(ProcessOrder))]
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
        [HttpPut(Name = nameof(ModifyOrder))]
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
        [HttpDelete("{orderId}", Name = nameof(CancellOrder))]
        public async Task<IActionResult> CancellOrder(long orderId)
        {
            try
            {
                var result = await cancellOrderCommandHandler.Handle(orderId);

                if (result != null)
                {

                    var tuples = new List<Tuple<string, string, string, object?>>();
                    tuples.Add(new(nameof(this.ModifyOrder), "self", "PUT", null));
                    tuples.Add(new(nameof(this.GetOrder), "self", "Get", new { orderId = result.OrderId }));
                    tuples.Add(new(nameof(this.ProcessOrder), "self", "POST", null));

                    return AcceptedAtAction(
                                           "CancellOrder",
                                           "Orders",
                                            null,
                                            new ProcessedOrderDto()
                                            {
                                                CancelledOrders = result.CancelledOrders,
                                                OrderId = orderId,
                                                Trades = result.Trades,
                                                Links = Links(tuples)
                                            });

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
        [HttpDelete(Name = nameof(CancellAllOrders))]
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

        [HttpGet("{orderId}", Name = nameof(GetOrder))]
        public async Task<IActionResult> GetOrder(long orderId)
        {
            return Ok(await orderQueryRepository.Get(o => o.Id == orderId));
        }

        private List<LinkDto> Links(List<Tuple<string, string, string, object?>> linkDtos)
        {
            var linkDto = new List<LinkDto>();

            foreach (var item in linkDtos)
            {
                linkDto.Add(new LinkDto(Url.RouteUrl(item.Item1, item.Item4), item.Item2, item.Item3));
            }

            return linkDto;
        }
    }
}
