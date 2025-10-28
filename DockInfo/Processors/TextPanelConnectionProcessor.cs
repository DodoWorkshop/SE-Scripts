namespace IngameScript
{
    public class TextPanelConnectionProcessor
    {
        private readonly GroupTextPanelService _groupTextPanelService;
        private readonly DockGroupRepository _groupRepository;

        public TextPanelConnectionProcessor(Program program)
        {
            _groupRepository = program.Container.GetItem<DockGroupRepository>();
            _groupTextPanelService = program.Container.GetItem<GroupTextPanelService>();

            var eventBus = program.Container.GetItem<IEventBus<IConnectorEvent>>();
            eventBus.RegisterConsumer(HandleConnectorEvent);
        }

        private void HandleConnectorEvent(IConnectorEvent connectorEvent)
        {
            var group = _groupRepository.GetGroup(connectorEvent.GroupName);
            if (group == null) return;

            _groupTextPanelService.RefreshPanels(group);
        }
    }
}