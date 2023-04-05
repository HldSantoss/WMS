namespace Domain.Adapters
{
    public interface IPublisher
    {
        void Publish(string message, string routeKey, string exchange);
    }
}