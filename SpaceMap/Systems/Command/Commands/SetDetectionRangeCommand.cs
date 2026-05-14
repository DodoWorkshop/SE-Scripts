using System;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class SetDetectionRangeCommand : ICommand
    {
        private readonly IUserSettingsRepository _userSettingsRepository;

        public SetDetectionRangeCommand(Program program)
        {
            _userSettingsRepository = program.Container.GetItem<IUserSettingsRepository>();
        }

        public string[] Names => new[] { "detect_range", "dr" };

        public string Execute(MyCommandLine commandLine)
        {
            if (commandLine.Items.Count < 2)
                throw new Exception("A value is required. Example: detect_range 8000");

            var arg = commandLine.Items[1];
            if (arg.Length > 0 && (arg[0] == '+' || arg[0] == '-'))
            {
                var delta = int.Parse(arg);
                var newValue = (long)_userSettingsRepository.DetectionDistance + delta;
                _userSettingsRepository.DetectionDistance = (uint)Math.Max(0L, newValue);
            }
            else
            {
                _userSettingsRepository.DetectionDistance = uint.Parse(arg);
            }

            return $"Detect range: {_userSettingsRepository.DetectionDistance}m";
        }

        public string GetUsage()
        {
            return "detect_range <meters|+delta|-delta>  alias: dr\n" +
                   "  Set or adjust the camera detection range.\n" +
                   "  Example: detect_range 8000 | detect_range +2000 | dr -1000";
        }
    }
}
