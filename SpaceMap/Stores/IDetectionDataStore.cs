using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public interface IDetectionDataStore : IStore
    {
        float RaycastCharge { get; set; }
        
        MyDetectedEntityInfo? DetectedEntityInfo { get; set; }
    }
}