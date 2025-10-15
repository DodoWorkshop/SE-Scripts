using System.Collections.Generic;

namespace IngameScript
{
    public class BlocDetectionTimer : IBlockDetectionTimer
    {
        private readonly Program _program;
        private readonly IEventSink<ISpaceMapEvent> _eventBus;

        private long _lastPerformedTick = -Program.SearchBlockInterval;

        public BlocDetectionTimer(Program program)
        {
            _program = program;
            _eventBus = program.EventBus;
        }

        public string Group => SystemGroups.Logic;

        public IEnumerator<bool> Run()
        {
            if (_program.Runtime.LifetimeTicks < _lastPerformedTick + Program.SearchBlockInterval)
            {
                yield return false;
                yield break;
            }

            _lastPerformedTick = _program.Runtime.LifetimeTicks;

            _eventBus.Produce(new BlocDetectionPulseEvent());
        }
    }
}