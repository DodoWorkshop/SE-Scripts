using System;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class LocalDatabaseSystem : IDatabaseSystem
    {
        private readonly IEventSink<ISpaceMapEvent> _eventSink;
        private readonly IMapEntryRepository _mapEntryRepository;

        public LocalDatabaseSystem(Program program)
        {
            _eventSink = program.Container.GetItem<IEventSink<ISpaceMapEvent>>();
            _mapEntryRepository = program.Container.GetItem<IMapEntryRepository>();

            var eventStream = program.Container.GetItem<IEventStream<ISpaceMapEvent>>();
            eventStream.RegisterConsumer(@event =>
            {
                if (@event is EntityDetectedEvent)
                {
                    HandleEntityDetectedEvent((EntityDetectedEvent)@event);
                }
            });
        }

        private void HandleEntityDetectedEvent(EntityDetectedEvent message)
        {
            var previousEntry = _mapEntryRepository.GetOneById<IMapEntry>(message.DetectedEntity.EntityId);
            if (previousEntry == null)
            {
                var entry = BuildEntryFromDetectedEntity(message.DetectedEntity);
                if (entry != null)
                {
                    _mapEntryRepository.Save(entry);
                    _eventSink.Produce(new NewMapEntryRegisteredEvent(entry));
                }
            }
        }

        private IMapEntry BuildEntryFromDetectedEntity(MyDetectedEntityInfo detectedEntity)
        {
            if (detectedEntity.Type == MyDetectedEntityType.Asteroid)
            {
                return new Asteroid(
                    detectedEntity.EntityId,
                    "A" + NameGenerator.Generate(detectedEntity.EntityId),
                    detectedEntity.Position
                )
                {
                    UpdateDate = DateTime.Now.Ticks
                };
            }

            return null;
        }

        public void Run(string argument, UpdateType updateSource)
        {
        }
    }
}