﻿namespace TradeMatchingEngine
{
    public partial class StockMarketMatchEngine
    {
        class StockMarketState : IStockMarketMatchEngine
        {
            public MarcketState Code;

            protected StockMarketMatchEngine StockMarketMatchEngine;

            public StockMarketState(StockMarketMatchEngine stockMarketMatchEngine)
            {
                this.StockMarketMatchEngine = stockMarketMatchEngine;
            }

            public virtual void PreOpen()
            {
                throw new NotImplementedException();
            }

            public virtual void Open()
            {
                throw new NotImplementedException();
            }

            public virtual void Close()
            {
                throw new NotImplementedException();
            }

            public virtual async Task<int> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, int? orderParentId = null)
            {
                throw new NotImplementedException();
            }
            public virtual async Task<int> CancellOrderAsync(int orderId)
            {
                throw new NotImplementedException();
            }
            public virtual async Task<int> ModifieOrder(int orderId, int price, int amount, DateTime expirationDate)
            {
                throw new NotImplementedException();
            }
            public virtual void ClearQueue()
            {
                throw new NotImplementedException();
            }
        }
        class Closed : StockMarketState
        {
            public Closed(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
            {
            }

            public override void PreOpen()
            {
                StockMarketMatchEngine.state = new PreOpened(StockMarketMatchEngine);
                StockMarketMatchEngine.state.Code = MarcketState.PreOpen;

                var stockMarketMatchEngineEvents = new StockMarketMatchEngineEvents()
                {
                    EventType = EventType.MarketPreOpened,
                    Description = "State Of Market Is Changed to PreOpened"
                };
                StockMarketMatchEngine.OnProcessCompleted(stockMarketMatchEngineEvents);
            }

            public override async Task<int> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null,int? orderParentId = null)
            {
                return await StockMarketMatchEngine.processOrderAsync(price, amount, side, expireTime, fillAndKill,orderParentId);
            }


            //public override void ClearQueue()
            //{
            //    StockMarketMatchEngine.clearQueue();
            //}
        }
        class Opened : StockMarketState
        {
            public Opened(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
            {
            }

            public override void PreOpen()
            {
                StockMarketMatchEngine.state = new PreOpened(StockMarketMatchEngine);
                StockMarketMatchEngine.state.Code = MarcketState.PreOpen;

                var stockMarketMatchEngineEvents = new StockMarketMatchEngineEvents()
                {
                    EventType = EventType.MarketPreOpened,
                    Description = "State Of Market Is Changed to PreOpened"
                };
                StockMarketMatchEngine.OnProcessCompleted(stockMarketMatchEngineEvents);
            }

            public override async Task<int> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, int? orderParentId = null)
            {
                return await StockMarketMatchEngine.processOrderAsync(price, amount, side, expireTime, fillAndKill,  orderParentId);
            }

            public override async Task<int> CancellOrderAsync(int orderId)
            {
                return StockMarketMatchEngine.cancellOrderAsync(orderId);
            }
            public override async Task<int> ModifieOrder(int orderId, int price, int amount, DateTime expirationDate)
            {
                return await StockMarketMatchEngine.modifieOrder(orderId, price, amount, expirationDate);
            }
        }
        class PreOpened : StockMarketState
        {
            public PreOpened(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
            {
            }

            public override void Close()
            {
                StockMarketMatchEngine.state = new Closed(StockMarketMatchEngine);
                StockMarketMatchEngine.state.Code = MarcketState.Close;

                var stockMarketMatchEngineEvents = new StockMarketMatchEngineEvents()
                {
                    EventType = EventType.MarketClosed,
                    Description = "State Of Market Is Changed to Closed"
                };
                StockMarketMatchEngine.OnProcessCompleted(stockMarketMatchEngineEvents);
            }

            public override void Open()
            {
                StockMarketMatchEngine.state = new Opened(StockMarketMatchEngine);
                StockMarketMatchEngine.state.Code = MarcketState.Open;

                var stockMarketMatchEngineEvents = new StockMarketMatchEngineEvents()
                {
                    EventType = EventType.MarketOpened,
                    Description = "State Of Market Is Changed to Opened"
                };
                StockMarketMatchEngine.OnProcessCompleted(stockMarketMatchEngineEvents);
            }

            public override async Task<int> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, int? orderParentId = null)
            {
                return await StockMarketMatchEngine.processOrderAsync(price, amount, side, expireTime, fillAndKill,orderParentId);
            }

            public override async Task<int> CancellOrderAsync(int orderId)
            {
                return StockMarketMatchEngine.cancellOrderAsync(orderId);
            }

            public override async Task<int> ModifieOrder(int orderId, int price, int amount, DateTime expirationDate)
            {
                return await StockMarketMatchEngine.modifieOrder(orderId, price, amount, expirationDate);
            }
        }
    }
}
