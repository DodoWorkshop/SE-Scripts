using System;

namespace IngameScript
{
    public class ConnectionHistoryProcessor : IContainerItem
    {
        private readonly DockGroupRepository _groupRepository;
        private readonly ConnectionHistoryRepository _historyRepository;

        public ConnectionHistoryProcessor(Program program)
        {
            _groupRepository = program.Container.GetItem<DockGroupRepository>();
            _historyRepository = program.Container.GetItem<ConnectionHistoryRepository>();

            var eventBus = program.Container.GetItem<IEventBus<IConnectorEvent>>();
            eventBus.RegisterConsumer(HandleConnectorEvent);
        }

        private void HandleConnectorEvent(IConnectorEvent connectorEvent)
        {
            var group = _groupRepository.GetGroup(@connectorEvent.GroupName);
            if (group == null) return;
            if (connectorEvent is ConnectorConnectedEvent)
            {
                HandleConnection(group);
            }
            else if (connectorEvent is ConnectorDisconnectedEvent)
            {
                HandleDisconnection(group);
            }
        }

        private void HandleConnection(DockGroup dockGroup)
        {
            var otherGrid = dockGroup.Connector.OtherConnector.CubeGrid;

            var entry = new ConnectionHistory(
                otherGrid.EntityId,
                otherGrid.CustomName,
                HistoryType.Landing,
                DateTime.Now,
                dockGroup.Name
            );

            _historyRepository.Register(entry);
        }

        private void HandleDisconnection(DockGroup dockGroup)
        {
            var relatedHistory = _historyRepository.GetLastLanding(dockGroup.Name);
            ConnectionHistory entry;
            if (relatedHistory.HasValue && relatedHistory.Value.GridId > 0)
            {
                entry = new ConnectionHistory(
                    relatedHistory.Value.GridId,
                    relatedHistory.Value.GridName,
                    HistoryType.Takeoff,
                    DateTime.Now,
                    dockGroup.Name
                );
            }
            else
            {
                entry = new ConnectionHistory(
                    -1,
                    "Unknown",
                    HistoryType.Takeoff,
                    DateTime.Now,
                    dockGroup.Name
                );
            }
            _historyRepository.Register(entry);
        }
    }
}