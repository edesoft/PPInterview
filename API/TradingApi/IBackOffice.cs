namespace TradingApi
{
    public interface IBackOffice
    {
        void BuyStock(string symbol, int quantity, decimal limitPrice);
    }
}