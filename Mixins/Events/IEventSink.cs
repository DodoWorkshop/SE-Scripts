namespace IngameScript
{
    public interface IEventSink<in T> where T : IEvent
    {
        void Produce(T @event);
    }
}