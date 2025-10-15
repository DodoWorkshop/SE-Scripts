namespace IngameScript
{
    public interface IEventSink<in T> : IContainerItem where T : IEvent
    {
        void Produce(T @event);
    }
}