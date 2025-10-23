using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class DetectionDataRepository : IDetectionDataRepository
    {
        public float RaycastCharge { get; set; }

        public MyDetectedEntityInfo? DetectedEntityInfo { get; set; }
    }
}