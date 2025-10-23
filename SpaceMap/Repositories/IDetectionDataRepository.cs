using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public interface IDetectionDataRepository : IRepository
    {
        float RaycastCharge { get; set; }

        MyDetectedEntityInfo? DetectedEntityInfo { get; set; }
    }
}