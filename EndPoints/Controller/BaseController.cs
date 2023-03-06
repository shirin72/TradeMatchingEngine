using Domain;
using EndPoints.Model;
using Microsoft.AspNetCore.Mvc;

namespace EndPoints.Controller
{

    public class BaseController : ControllerBase
    {
        #region Const
        private static readonly string HTTPMETHOD_GET = "GET";
        private static readonly string HTTPMETHOD_POST = "POST";
        private static readonly string HTTPMETHOD_PUT = "PUT";
        private static readonly string HTTPMETHOD_DELETE = "DELETE";
        private static readonly string REL_CANCELL_ORDER = "cancell_order";
        private static readonly string REL_GET_ORDER = "get_order";
        private static readonly string REL_GET_TRADE = "get_trade";
        private static readonly string REL_PUT_ORDER = "modify_order";
        private static readonly string REL_POST_ORDER = "post_order";
        #endregion

        protected ProcessedOrderVM CreateResponse(ProcessedOrder? result, List<Tuple<string, string, string, object?>> linkDtos)
        {
            List<TradeVM> tradeList = ConvertItradToTradeVM(result, linkDtos);

            List<TradeVM> trades = CreateTradesLink(tradeList, linkDtos.Where(l => l.Item1 == nameof(TradesController.GetTrade)).ToList());

            return CreateProcessedOrder(result, trades, linkDtos.Where(l => l.Item1 != nameof(TradesController.GetTrade)).ToList());
        }

        private List<TradeVM> ConvertItradToTradeVM(ProcessedOrder? result, List<Tuple<string, string, string, object>> linkDtos)
        {
            var tradeList = new List<TradeVM>();

            if (result.Trades != null)
            {
                foreach (var item in result.Trades)
                {
                    tradeList.Add(new TradeVM()
                    {
                        Amount = item.Amount,
                        BuyOrderId = item.BuyOrderId,
                        Id = item.Id,
                        Price = item.Price,
                        SellOrderId = item.SellOrderId,
                    });
                }

            }

            return tradeList;
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
        protected List<TradeVM> CreateTradesLink(List<TradeVM>? trades, List<Tuple<string, string, string, object?>> linkDtos)
        {
            if (trades != null)
            {
                trades.ForEach(t => t.Link = CreateLinks(linkDtos));
            }

            return trades;
        }
        protected List<Tuple<string, string, string, object?>> addAllowedProcessOrderLinks(List<Tuple<string, string, string, object?>> tuples, IEnumerable<ITrade> trades = null, long? orderId = null)
        {
            tuples.Add(new(nameof(OrdersController.CancellOrder), REL_CANCELL_ORDER, HTTPMETHOD_DELETE, orderId));
            tuples.Add(new(nameof(OrdersController.GetOrder), REL_GET_ORDER, HTTPMETHOD_GET, orderId));
            tuples.Add(new(nameof(OrdersController.ModifyOrder), REL_PUT_ORDER, HTTPMETHOD_PUT, null));

            if (trades != null)
            {
                addTradeLink(tuples, trades);
            }

            return tuples;
        }
        protected List<Tuple<string, string, string, object?>> addAllowedModifyOrderLinks(List<Tuple<string, string, string, object?>> tuples, IEnumerable<ITrade> trades = null, long? orderId = null)
        {
            tuples.Add(new(nameof(OrdersController.CancellOrder), REL_CANCELL_ORDER, HTTPMETHOD_DELETE, orderId));
            tuples.Add(new(nameof(OrdersController.GetOrder), REL_GET_ORDER, HTTPMETHOD_GET, orderId));
            tuples.Add(new(nameof(OrdersController.ProcessOrder), REL_POST_ORDER, HTTPMETHOD_POST, null));

            if (trades != null)
            {
                addTradeLink(tuples, trades);
            }

            return tuples;
        }

        protected void addTradeLink(List<Tuple<string, string, string, object>> tuples, IEnumerable<ITrade> trades)
        {
            foreach (var trade in trades)
            {
                tuples.Add(new(nameof(TradesController.GetTrade), REL_GET_TRADE, HTTPMETHOD_GET, trade.Id));
            }
        }

        protected List<Tuple<string, string, string, object?>> addAllowedGetOrderLinks(List<Tuple<string, string, string, object?>> tuples, IEnumerable<ITrade> trades = null, long? orderId = null)
        {
            tuples.Add(new(nameof(OrdersController.CancellOrder), REL_CANCELL_ORDER, HTTPMETHOD_DELETE, orderId));
            tuples.Add(new(nameof(OrdersController.ModifyOrder), REL_PUT_ORDER, HTTPMETHOD_PUT, null));
            return tuples;
        }
        protected List<Tuple<string, string, string, object?>> createTupleObject()
        {
            return new List<Tuple<string, string, string, object?>>();
        }
    }
}
