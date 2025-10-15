namespace IngameScript
{
    public interface IConnectorEvent : IEvent
    {
        string GroupName { get; }
    }
}