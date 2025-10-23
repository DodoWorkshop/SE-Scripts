using VRageMath;

namespace IngameScript
{
    public class DistanceTriggerState : ITriggerState
    {
        public DistanceTriggerState(Trigger trigger)
        {
            Trigger = trigger;
        }

        public Trigger Trigger { get; }

        public Vector3D Origin;

        public float Distance;
    }
}