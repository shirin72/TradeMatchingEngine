namespace TradeMatchingEngine
{
    public class ModifiedOrderPriorityMax : IComparer<PriceInQueue>
    {
        public int Compare(PriceInQueue? x, PriceInQueue? y)
        {
          if (x.Price < y.Price)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
