namespace IngameScript
{
    public class DurationTriggerState : ITriggerState
    {
        public DurationTriggerState(Trigger trigger)
        {
            Trigger = trigger;
        }

        public Trigger Trigger { get; }

        public long DestinationTick;
        
        public long CurrentTick;
    }
}