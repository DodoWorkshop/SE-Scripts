using System;

namespace IngameScript
{
    public interface IEventStream<out T> : IContainerItem where T : IEvent
    {
        uint RegisterConsumer(Action<T> consumer);

        void UnregisterConsumer(uint consumerId);
    }
}