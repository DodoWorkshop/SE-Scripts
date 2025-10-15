namespace IngameScript
{
    public interface IEventBus<T> : IEventSink<T>, IEventStream<T> where T : IEvent
    {
    }
}