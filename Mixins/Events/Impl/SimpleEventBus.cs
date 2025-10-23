using System;
using System.Collections.Generic;

namespace IngameScript
{
    public class SimpleEventBus<T> : IEventBus<T> where T : IEvent
    {
        private readonly List<IConsumer> _consumers = new List<IConsumer>();
        private uint _idSequence;

        public void Produce(T message)
        {
            foreach (var consumer in _consumers)
            {
                consumer.Handle(message);
            }
        }

        public uint RegisterConsumer(Action<T> handler)
        {
            _idSequence++;
            var consumer = new Consumer(_idSequence, handler);
            _consumers.Add(consumer);
            return consumer.Id;
        }

        public void UnregisterConsumer(uint consumerId)
        {
            _consumers.RemoveAll(x => x.Id == consumerId);
        }

        private interface IConsumer
        {
            uint Id { get; }

            void Handle(IEvent message);
        }

        private struct Consumer : IConsumer
        {
            public uint Id { get; }

            private readonly Action<T> _handler;

            public Consumer(uint id, Action<T> handler)
            {
                Id = id;
                _handler = handler;
            }

            public void Handle(IEvent message)
            {
                if (message is T)
                {
                    _handler((T)message);
                }
            }
        }
    }
}