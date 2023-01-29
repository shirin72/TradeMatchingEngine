namespace TradeMatchingEngine
{
    //public class StockMarketEngineProxy 
    //{
    //    private IStockMarketMatchEngine _stockMarketMatchEngine;

    //    protected StockMarketEngineProxy state;

    //    protected MarcketState _code;

    //    public MarcketState StateCode => _code;

    //    public StockMarketEngineProxy()
    //    {
    //        state = new Closed(_stockMarketMatchEngine as StockMarketMatchEngine);
    //    }

    //    public StockMarketEngineProxy(IStockMarketMatchEngine stockMarketMatchEngine)
    //    {
    //        _stockMarketMatchEngine = stockMarketMatchEngine;
    //    }

    //    public virtual async Task<long?> CancelOrderAsync(long orderId, StockMarketEvents? events = null)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public virtual void Close()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async ValueTask DisposeAsync()
    //    {
    //        await _stockMarketMatchEngine.DisposeAsync();
    //    }

    //    public virtual async Task<long?> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events = null)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public virtual void Open()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public virtual void PreOpen()
    //    {
    //        state = new PreOpened(_stockMarketMatchEngine as StockMarketMatchEngine);
    //    }

    //    public virtual Task<long> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null, StockMarketEvents? events = null)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    //class Closed : StockMarketEngineProxy
    //{
    //    private readonly StockMarketMatchEngine _stockMarketMatchEngine;
    //    public Closed(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
    //    {
    //        _stockMarketMatchEngine = stockMarketMatchEngine;
    //    }

    //    public override void PreOpen()
    //    {
    //        this.state = new PreOpened(_stockMarketMatchEngine);
    //        this._code = MarcketState.PreOpen;
    //    }
    //}
    //class Opened : StockMarketEngineProxy
    //{
    //    private readonly StockMarketMatchEngine _stockMarketMatchEngine;
    //    public Opened(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
    //    {
    //        _stockMarketMatchEngine = stockMarketMatchEngine;
    //    }

    //    public override void PreOpen()
    //    {
    //        this.state = new PreOpened(_stockMarketMatchEngine);
    //        this._code = MarcketState.PreOpen;
    //    }

    //    public override async Task<long> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null, StockMarketEvents? events = null)
    //    {
    //        return await _stockMarketMatchEngine.ProcessOrderAsync(price, amount, side, expireTime, fillAndKill, orderParentId, events);
    //    }

    //    public override async Task<long?> CancelOrderAsync(long orderId, StockMarketEvents? events = null)
    //    {
    //        return await _stockMarketMatchEngine.CancelOrderAsync(orderId, events);
    //    }
    //    public override async Task<long?> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events = null)
    //    {
    //        return await _stockMarketMatchEngine.ModifieOrder(orderId, price, amount, expirationDate, events);
    //    }
    //}
    //class PreOpened : StockMarketEngineProxy
    //{
    //    private readonly StockMarketMatchEngine _stockMarketMatchEngine;
    //    public PreOpened(StockMarketMatchEngine stockMarketMatchEngine) : base(stockMarketMatchEngine)
    //    {
    //        _stockMarketMatchEngine=stockMarketMatchEngine;
    //    }

    //    public override void Close()
    //    {
    //        this.state = new Closed(_stockMarketMatchEngine);
    //        this._code = MarcketState.Close;

    //    }

    //    public override void Open()
    //    {
    //        this.state =  new Opened(_stockMarketMatchEngine);
    //        this._code = MarcketState.Open;
    //    }

    //    public override async Task<long> ProcessOrderAsync(int price, int amount, Side side, DateTime? expireTime = null, bool? fillAndKill = null, long? orderParentId = null, StockMarketEvents? events = null)
    //    {
    //        return await _stockMarketMatchEngine.preProcessOrderAsync(price, amount, side, expireTime, fillAndKill, orderParentId, events);
    //    }

    //    public override async Task<long?> CancelOrderAsync(long orderId, StockMarketEvents? events = null)
    //    {
    //        return await _stockMarketMatchEngine.cancelOrderAsync(orderId, events);
    //    }

    //    public override async Task<long?> ModifieOrder(long orderId, int price, int amount, DateTime? expirationDate, StockMarketEvents? events = null)
    //    {
    //        return await _stockMarketMatchEngine.modifieOrder(orderId, price, amount, expirationDate, events);
    //    }
    //}
}
