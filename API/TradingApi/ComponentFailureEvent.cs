using System;

namespace TradingApi
{
    public class FailureEventArgs : EventArgs
    {
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public decimal MarketPrice { get; set; }
        public string Error { get; set; }

        public FailureEventArgs(string symbol, int quantity, decimal marketPrice, string error)
        {
            this.Symbol = symbol;
            this.Quantity = quantity;
            this.MarketPrice = marketPrice;
            this.Error = error;
        }
    }

    public delegate void ComponentFailureEvent(object sender, FailureEventArgs args);
}