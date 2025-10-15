namespace IngameScript
{
    public struct ConnectorDisconnectedEvent : IConnectorEvent
    {
        public string GroupName { get; }

        public ConnectorDisconnectedEvent(string groupName)
        {
            GroupName = groupName;
        }
    }
}