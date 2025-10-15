using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class ConnectionTriggerSystem : IRuntimeSystem
    {
        private readonly DockGroupRepository _groupRepository;
        private readonly IEventSink<IConnectorEvent> _eventBus;

        public ConnectionTriggerSystem(Program program)
        {
            _groupRepository = program.Container.GetItem<DockGroupRepository>();
            _eventBus = program.Container.GetItem<IEventBus<IConnectorEvent>>();
        }

        public void Run(string argument, UpdateType updateSource)
        {
            foreach (var group in _groupRepository.GetAll())
            {
                var connectorConnected = group.Connector.IsConnected;
                if (connectorConnected != group.IsConnected)
                {
                    group.IsConnected = connectorConnected;
                    if (connectorConnected)
                    {
                        _eventBus.Produce(new ConnectorConnectedEvent(group.Name));
                    }
                    else
                    {
                        _eventBus.Produce(new ConnectorDisconnectedEvent(group.Name));
                    }
                }
            }
        }
    }
}