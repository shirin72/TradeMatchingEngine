namespace Domain
{
    public interface IStockMarketMatchingEngineProcessContext
    {
        IEnumerable<ITrade> CreatedTrades { get;  }
        IEnumerable<IOrder> ModifiedOrders { get;  }
        IOrder? Order { get;  }
    }
}