namespace IngameScript
{
    public struct ConnectorConnectedEvent : IConnectorEvent
    {
        public string GroupName { get; }

        public ConnectorConnectedEvent(string groupName)
        {
            GroupName = groupName;
        }
    }
}