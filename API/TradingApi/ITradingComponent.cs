namespace TradingApi
{
    public interface ITradingComponent
    {
        event ComponentCompletionEvent CompletionEvent;
        event ComponentFailureEvent FailureEvent;
    }
}