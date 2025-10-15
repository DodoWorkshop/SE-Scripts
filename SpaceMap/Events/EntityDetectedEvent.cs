using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public struct EntityDetectedEvent : ISpaceMapEvent
    {
        public MyDetectedEntityInfo DetectedEntity { get; }
        
        public EntityDetectedEvent(MyDetectedEntityInfo detectedEntity)
        {
            DetectedEntity = detectedEntity;
        }
    }
}