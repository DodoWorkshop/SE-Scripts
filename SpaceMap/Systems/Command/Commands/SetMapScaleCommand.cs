using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class SetMapScaleCommand : ICommand
    {
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly Program _program;

        public SetMapScaleCommand(Program program)
        {
            _userSettingsRepository = program.RepositoryManager.GetRepository<IUserSettingsRepository>();
            _program = program;
        }
        
        public string[] Names => new [] { "map_scale_set", "mss" };
        
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