﻿namespace TradeMatchingEngine
{
    public class Order
    {
        public int Id { get; set; }

        public Side Side { get; set; }

        public int Price { get; set; }

        public int Amount { get; set; }
    }
}
