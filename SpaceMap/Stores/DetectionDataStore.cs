using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class DetectionDataStore : IDetectionDataStore
    {
        public float RaycastCharge { get; set; }
        
        public MyDetectedEntityInfo? DetectedEntityInfo { get; set; }
    }
}