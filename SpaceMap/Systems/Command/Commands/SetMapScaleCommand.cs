using System;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class SetMapScaleCommand : ICommand
    {
        private readonly IUserSettingsRepository _userSettingsRepository;

        public SetMapScaleCommand(Program program)
        {
            _userSettingsRepository = program.Container.GetItem<IUserSettingsRepository>();
        }

        public string[] Names => new[] { "map_scale", "ms" };

        public string Execute(MyCommandLine commandLine)
        {
            if (commandLine.Items.Count < 2)
                throw new Exception("A value is required. Example: map_scale 5000");

            var arg = commandLine.Items[1];
            if (arg.Length > 0 && (arg[0] == '+' || arg[0] == '-'))
            {
                var delta = int.Parse(arg);
                var newValue = (long)_userSettingsRepository.MapScale + delta;
                _userSettingsRepository.MapScale = (uint)Math.Max(0L, newValue);
            }
            else
            {
                _userSettingsRepository.MapScale = uint.Parse(arg);
            }

            return $"Map scale: {_userSettingsRepository.MapScale}m";
        }

        public string GetUsage()
        {
            return "map_scale <meters|+delta|-delta>  alias: ms\n" +
                   "  Set or adjust the radar display radius (Map and Map3D views).\n" +
                   "  Example: map_scale 5000 | map_scale +1000 | ms -500";
        }
    }
}