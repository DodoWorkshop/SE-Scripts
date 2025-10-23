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

        public string[] Names => new[] { "map_scale_set", "mss" };

        public void Execute(MyCommandLine commandLine)
        {
            var scale = uint.Parse(commandLine.Argument(1));
            _userSettingsRepository.MapScale = scale;
        }

        public string GetUsage()
        {
            return "TODO";
        }
    }
}