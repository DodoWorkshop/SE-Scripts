namespace IngameScript
{
    public struct NewMapEntryRegisteredEvent : ISpaceMapEvent
    {
        public IMapEntry MapEntry { get; }
        
        public NewMapEntryRegisteredEvent(IMapEntry mapEntry)
        {
            MapEntry = mapEntry;
        }
    }
}