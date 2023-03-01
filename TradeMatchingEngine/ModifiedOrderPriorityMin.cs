namespace Domain
{
    public class ModifiedOrderPriorityMin : IComparer<Order>
    {
        public int Compare(Order? x, Order? y)
        {
            if (x.Price == y.Price)
            {
                if (x.Id < y.Id)
                {
                    return -1;
                }
                else 
                {
                    return 1;
                }

            }
            else if (x.Price > y.Price)
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
