using System;
using TradingApi;

namespace TradingComponent
{
    public class StockPurchaseBot : ITradingComponent, IPriceConsumer, IDisposable
    {
        public event ComponentCompletionEvent CompletionEvent;
        public event ComponentFailureEvent FailureEvent;

        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public decimal ThresholdPrice { get; set; }

        public IBackOffice BackOffice { get; set; }

        public StockPurchaseBot(string symbol, int quantity, decimal thresholdPrice)
        {
            this.Symbol = symbol;
            this.Quantity = quantity;
            this.ThresholdPrice = thresholdPrice;
        }

        public virtual void OnCompletion(CompletionEventArgs e)
        {
            CompletionEvent?.Invoke(this, e);
        }

        public virtual void OnFailure(FailureEventArgs e)
        {
            FailureEvent?.Invoke(this, e);
        }

        public void ReceivePriceTick(string symbol, decimal marketPrice)
        {
            if (symbol == this.Symbol && marketPrice < this.ThresholdPrice)
            {
                // check back office
                if (this.BackOffice == null)
                {
                    OnFailure(new FailureEventArgs(this.Symbol, this.Quantity, marketPrice, "BackOffice is not initialized"));
                    return;
                }

                // buy stock
                try
                {
                    this.BackOffice.BuyStock(this.Symbol, this.Quantity, marketPrice);
                    OnCompletion(new CompletionEventArgs(this.Symbol, this.Quantity, marketPrice));
                }
                catch (Exception e)
                {
                    OnFailure(new FailureEventArgs(this.Symbol, this.Quantity, marketPrice, e.Message));
                }
            }
        }

        public void Dispose()
        {
            // to dispose any resource
        }
    }
}
