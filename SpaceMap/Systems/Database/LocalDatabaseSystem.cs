using System;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class LocalDatabaseSystem : IDatabaseSystem
    {
        private readonly Program _program;
        private readonly IEventBus<ISpaceMapEvent> _eventBus;
        private readonly IMapEntryRepository _mapEntryRepository;

        public LocalDatabaseSystem(Program program)
        {
            _program = program;
            _eventBus = program.EventBus;
            _mapEntryRepository = program.RepositoryManager.GetRepository<IMapEntryRepository>();

            _eventBus.RegisterConsumer(@event =>
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

                    _eventBus.Produce(new NewMapEntryRegisteredEvent(entry));
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