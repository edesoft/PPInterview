using System;
using System.Collections.Generic;

namespace TradingApi
{
    public interface IPriceConsumer 
    {
        void ReceivePriceTick(string symbol, decimal marketPrice);
    }
}