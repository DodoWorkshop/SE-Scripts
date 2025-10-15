using System;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class TimerBlocConnectionProcessor : IContainerItem
    {
        private readonly Program _program;
        private readonly MyIni _ini = new MyIni();
        private readonly DockGroupRepository _groupRepository;

        public TimerBlocConnectionProcessor(Program program)
        {
            _program = program;
            _groupRepository = program.Container.GetItem<DockGroupRepository>();

            var eventBus = program.Container.GetItem<IEventBus<IConnectorEvent>>();
            eventBus.RegisterConsumer(HandleConnectorEvent);
        }

        private void HandleConnectorEvent(IConnectorEvent connectorEvent)
        {
            var group = _groupRepository.GetGroup(connectorEvent.GroupName);
            if (group == null) return;

            var section = connectorEvent is ConnectorConnectedEvent
                ? DataStructure.ConnectedSection
                : DataStructure.DisconnectedSection;
            foreach (var timerBlock in group.TimerBlocks)
            {
                if (_ini.TryParse(timerBlock.CustomData))
                {
                    var actionStr = _ini.Get(section, DataStructure.TimerBlocActionField)
                        .ToString();

                    TimerBlocAction action;
                    if (Enum.TryParse(actionStr, true, out action))
                    {
                        switch (action)
                        {
                            case TimerBlocAction.Start:
                                timerBlock.StartCountdown();
                                break;
                            case TimerBlocAction.Stop:
                                timerBlock.StopCountdown();
                                break;
                            case TimerBlocAction.Trigger:
                                timerBlock.Trigger();
                                break;
                        }
                    }
                    else
                    {
                        _program.Echo($"Failed to read timer bloc action {actionStr}");
                    }
                }
            }
        }
    }
}