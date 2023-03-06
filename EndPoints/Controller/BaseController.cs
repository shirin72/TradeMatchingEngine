using Domain;
using EndPoints.Model;
using Microsoft.AspNetCore.Mvc;

namespace EndPoints.Controller
{

    public class BaseController : ControllerBase
    {
        protected ProcessedOrderVM CreateResponse(ProcessedOrder? result, List<Tuple<string, string, string, object?>> linkDtos)
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
        protected ProcessedOrderVM CreateProcessedOrder(ProcessedOrder? result, List<TradeVM> trades, List<Tuple<string, string, string, object?>> linkDtos)
        {
            return new ProcessedOrderVM()
            {
                CancelledOrders = result.CancelledOrders,
                RegisteredOrder = CreateRegisteredOrder(result.OrderId, linkDtos),
                Trades = trades,
            };
        }
        protected RegisteredOrderVM CreateRegisteredOrder(long orderId, List<Tuple<string, string, string, object?>> linkDtos)
        {
            return new RegisteredOrderVM()
            {
                OrderId = orderId,
                Links = CreateLinks(linkDtos)
            };
        }
        protected List<LinkVM> CreateLinks(List<Tuple<string, string, string, object?>> linkDtos)
        {
            var links = new List<LinkVM>();

            foreach (var link in linkDtos)
            {
                links.Add(new LinkVM(Url.Link(link.Item1, new { id = link.Item4 }), link.Item2, link.Item3));
            }

            return links;
        }
        protected List<TradeVM> CreateTrades(List<TradeVM>? trades, List<Tuple<string, string, string, object?>> linkDtos)
        {
            if (trades != null)
            {
                trades.ForEach(t => t.Link = CreateLinks(linkDtos));
            }

            return trades;
        }
        protected List<Tuple<string, string, string, object?>> addAllowedProcessOrderLinks(List<Tuple<string, string, string, object?>> tuples, IEnumerable<ITrade> trades = null, long? orderId = null)
        {
            tuples.Add(new(nameof(OrdersController.CancellOrder), "self", "DELETE", orderId));
            tuples.Add(new(nameof(OrdersController.GetOrder), "self", "Get", orderId));
            tuples.Add(new(nameof(OrdersController.ModifyOrder), "self", "PUT", null));

            foreach (var trade in trades)
            {
                tuples.Add(new(nameof(TradesController.GetTrade), "self", "Get", trade.Id));
            }

            return tuples;
        }
        protected List<Tuple<string, string, string, object?>> addAllowedModifyOrderLinks(List<Tuple<string, string, string, object?>> tuples, IEnumerable<ITrade> trades = null, long? orderId = null)
        {
            tuples.Add(new(nameof(OrdersController.CancellOrder), "self", "DELETE", orderId));
            tuples.Add(new(nameof(OrdersController.GetOrder), "self", "Get", orderId));
            tuples.Add(new(nameof(OrdersController.ProcessOrder), "self", "Post", null));
            if (trades != null)
            {
                foreach (var trade in trades)
                {
                    tuples.Add(new(nameof(TradesController.GetTrade), "self", "Get", trade.Id));
                }
            }

            return tuples;
        }
        protected List<Tuple<string, string, string, object?>> addAllowedGetOrderLinks(List<Tuple<string, string, string, object?>> tuples, IEnumerable<ITrade> trades = null, long? orderId = null)
        {
            tuples.Add(new(nameof(OrdersController.CancellOrder), "self", "DELETE", orderId));
            tuples.Add(new(nameof(OrdersController.ProcessOrder), "self", "Post", null));
            return tuples;
        }
        protected List<Tuple<string, string, string, object?>> createTupleObject()
        {
            return new List<Tuple<string, string, string, object?>>();
        }
    }
}
