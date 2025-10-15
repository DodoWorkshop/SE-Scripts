using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class HistoryDisplay
    {
        public HistoryDisplay(IMyTextPanel panel)
        {
            Panel = panel;
        }

        public IMyTextPanel Panel { get; }
    }
}