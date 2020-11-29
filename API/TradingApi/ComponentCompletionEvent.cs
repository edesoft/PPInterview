using System;

namespace TradingApi
{
    public class CompletionEventArgs : EventArgs
    {
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public decimal MarketPrice { get; set; }

        public CompletionEventArgs(string symbol, int quantity, decimal marketPrice)
        {
            this.Symbol = symbol;
            this.Quantity = quantity;
            this.MarketPrice = marketPrice;
        }
    }

    public delegate void ComponentCompletionEvent(object sender, CompletionEventArgs args);
}