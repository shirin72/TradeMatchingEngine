using Application.OrderService.OrderCommandHandlers;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeMatchingEngine;

namespace EndPoints.Controller
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IAddOrderCommandHandlers addOrderCommandHandlers;
        private readonly TradeMatchingEngineContext tradeMatchingEngineContext;


        public OrderController(IAddOrderCommandHandlers addOrderCommandHandlers)
        {
            this.addOrderCommandHandlers = addOrderCommandHandlers;
            tradeMatchingEngineContext = new TradeMatchingEngineContext(new Microsoft.EntityFrameworkCore.DbContextOptions<TradeMatchingEngineContext>());
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
            try
            {
                expDate = DateTime.Now.AddDays(1);

                var result = await addOrderCommandHandlers.Handle(price, amount, side, expDate, isFillAndKill);

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
           
            }
        }

        [HttpPost]
        public async Task<object> CreateOrder(int id)
        {
            try
            {
                var s = await tradeMatchingEngineContext.Orders.ToListAsync();
                //var order = new Order(id, Side.Buy, 100, 5, DateTime.Now.AddDays(1));
                //var s1 = tradeMatchingEngineContext.Orders.Add(order);
                //var s2 = tradeMatchingEngineContext.SaveChanges();
                return s;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
       
            }

        } 

       
    }
}
