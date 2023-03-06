using Application.OrderService.OrderCommandHandlers;
using Domain;
using Domain.Orders.Commands;
using Domain.Orders.Repositories.Query;
using EndPoints.Model;
using Microsoft.AspNetCore.Mvc;

namespace EndPoints.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : BaseController
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
        [HttpPost(Name = nameof(ProcessOrder))]
        public async Task<IActionResult> ProcessOrder([FromBody] RegisterOrderVM orderVM)
        {
            var command = new AddOrderCommand()
            {
                Amount = orderVM.Amount,
                ExpDate = orderVM.ExpireTime,
                Side = orderVM.Side,
                Price = orderVM.Price,
                IsFillAndKill = (bool)orderVM.IsFillAndKill,
            };

            var result = await addOrderCommandHandlers.Handle(command);

            return CreatedAtAction(
               "ProcessOrder",
                "Orders",
                null,
                CreateResponse(result, addAllowedProcessOrderLinks(createTupleObject(), result.Trades, result.OrderId))
               );
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

                return AcceptedAtAction(
                "ModifyOrder",
                "Orders",
                null,
                CreateResponse(result, addAllowedModifyOrderLinks(createTupleObject(), result.Trades, result.OrderId)));
            }

            return BadRequest(modifieOrderVM);
        }

        /// <summary>
        /// CancellOrder
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = nameof(CancellOrder))]
        public async Task<IActionResult> CancellOrder(long id)
        {
            try
            {
                var result = await cancellOrderCommandHandler.Handle(id);

                if (result != null)
                {
                    return AcceptedAtAction(
                                            "CancellOrder",
                                            "Orders",
                                            null,
                                            result);
                }

                return BadRequest(id);
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
                       null,
                       result);
            }

            return BadRequest();
        }

        [HttpGet("{id}", Name = nameof(GetOrder))]
        public async Task<IActionResult> GetOrder(long id)
        {
            var result = await orderQueryRepository.Get(o => o.Id == id);

            return Ok(new OrderVM()
            {
                Amount = result.Amount,
                ExpireTime = result.ExpireTime,
                Id = id,
                IsFillAndKill = result.IsFillAndKill,
                Price = result.Price,
                Side = result.Side,
                HasCompleted = result.HasCompleted,
                IsExpired = result.IsExpired,
                OrderParentId = result.OrderParentId,
                OrderState = result.OrderState,
                OriginalAmount = result.OriginalAmount,
                Links = CreateLinks(addAllowedGetOrderLinks(createTupleObject(), orderId: result.Id))
            });
        }

        #region Private
        private ProcessedOrderVM CreateResponse(ProcessedOrder? result, List<Tuple<string, string, string, object?>> linkDtos)
        {
            var lst = new List<TradeVM>();
            if (result.Trades != null)
            {
                foreach (var item in result.Trades)
                {
                    lst.Add(new TradeVM()
                    {
                        Amount = item.Amount,
                        BuyOrderId = item.BuyOrderId,
                        Id = item.Id,
                        Price = item.Price,
                        SellOrderId = item.SellOrderId,
                    });
                }
                List<TradeVM> trades = CreateTrades(lst, linkDtos.Where(l => l.Item1 == nameof(TradesController.GetTrade)).ToList());
            }

            return CreateProcessedOrder(result, lst, linkDtos.Where(l => l.Item1 != nameof(TradesController.GetTrade)).ToList());
        }
        private ProcessedOrderVM CreateProcessedOrder(ProcessedOrder? result, List<TradeVM> trades, List<Tuple<string, string, string, object?>> linkDtos)
        {
            return new ProcessedOrderVM()
            {
                CancelledOrders = result.CancelledOrders,
                RegisteredOrder = CreateRegisteredOrder(result.OrderId, linkDtos),
                Trades = trades,
            };
        }
        private RegisteredOrderVM CreateRegisteredOrder(long orderId, List<Tuple<string, string, string, object?>> linkDtos)
        {
            return new RegisteredOrderVM()
            {
                OrderId = orderId,
                Links = CreateLinks(linkDtos)
            };
        }
        private List<LinkVM> CreateLinks(List<Tuple<string, string, string, object?>> linkDtos)
        {
            var links = new List<LinkVM>();

            foreach (var link in linkDtos)
            {
                links.Add(new LinkVM(Url.Link(link.Item1, new { id = link.Item4 }), link.Item2, link.Item3));
            }

            return links;
        }
        private List<TradeVM> CreateTrades(List<TradeVM>? trades, List<Tuple<string, string, string, object?>> linkDtos)
        {
            if (trades != null)
            {
                trades.ForEach(t => t.Link = CreateLinks(linkDtos));
            }

            return trades;
        }

        private List<Tuple<string, string, string, object?>> addAllowedProcessOrderLinks(List<Tuple<string, string, string, object?>> tuples, IEnumerable<ITrade> trades = null, long? orderId = null)
        {
            tuples.Add(new(nameof(this.CancellOrder), "self", "DELETE", orderId));
            tuples.Add(new(nameof(this.GetOrder), "self", "Get", orderId));
            tuples.Add(new(nameof(this.ModifyOrder), "self", "PUT", null));

            foreach (var trade in trades)
            {
                tuples.Add(new(nameof(TradesController.GetTrade), "self", "Get", trade.Id));
            }

            return tuples;
        }
        private List<Tuple<string, string, string, object?>> addAllowedModifyOrderLinks(List<Tuple<string, string, string, object?>> tuples, IEnumerable<ITrade> trades = null, long? orderId = null)
        {
            tuples.Add(new(nameof(this.CancellOrder), "self", "DELETE", orderId));
            tuples.Add(new(nameof(this.GetOrder), "self", "Get", orderId));
            tuples.Add(new(nameof(this.ProcessOrder), "self", "Post", null));
            if (trades != null)
            {
                foreach (var trade in trades)
                {
                    tuples.Add(new(nameof(TradesController.GetTrade), "self", "Get", trade.Id));
                }
            }

            return tuples;
        }

        private List<Tuple<string, string, string, object?>> addAllowedGetOrderLinks(List<Tuple<string, string, string, object?>> tuples, IEnumerable<ITrade> trades = null, long? orderId = null)
        {
            tuples.Add(new(nameof(this.CancellOrder), "self", "DELETE", orderId));
            tuples.Add(new(nameof(this.ProcessOrder), "self", "Post", null));
            return tuples;
        }
        #endregion


    }
}
