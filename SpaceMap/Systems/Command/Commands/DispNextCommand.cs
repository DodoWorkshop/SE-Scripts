using System;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class DispNextCommand : ICommand
    {
        private readonly DisplayCycleSettings _cycleSettings;
        private readonly IhmSystem _ihmSystem;

        public DispNextCommand(Program program)
        {
            _cycleSettings = program.Container.GetItem<DisplayCycleSettings>();
            _ihmSystem = program.Container.GetItem<IhmSystem>();
        }

        public string[] Names => new[] { "disp_next", "dn" };

        public void Execute(MyCommandLine commandLine)
        {
            if (commandLine.ArgumentCount > 1)
            {
                _cycleSettings.CurrentMode = (DisplayMode)Enum.Parse(
                    typeof(DisplayMode), commandLine.Argument(1), true
                );
            }
            else
            {
                var order = DisplayCycleSettings.CycleOrder;
                var idx = Array.IndexOf(order, _cycleSettings.CurrentMode);
                _cycleSettings.CurrentMode = order[(idx + 1) % order.Length];
            }

            _ihmSystem.RefreshCycleSurfaces();
        }

        public string GetUsage()
        {
            return "disp_next [mode]  alias: dn\n" +
                   "  Cycle the display mode on screens tagged [SM:Cycle], or jump to a specific mode.\n" +
                   "  Modes: General, Map, Map3D, Database, Detection\n" +
                   "  Example: disp_next | disp_next Map3D";
        }
    }
}
