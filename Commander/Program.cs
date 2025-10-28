using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        #region mdk preserve

        // [Settings]----------------------------------------------------------------------------------

        // Logs
        public const bool LogCommandInfo = true;

        public const bool LogExecution = true;

        // [Execution]---------------------- Don't touch code below ------------------------------------

        #endregion

        public readonly IItemContainer Container;

        private readonly SystemManager _systemManager;

        public Program()
        {
            // Create container
            Container = new ItemContainer();

            // Register items
            Container.RegisterItem(BuildCommandRepository());
            Container.RegisterItem(new CommandReader());
            Container.RegisterItem(new MyCommandLine());

            // Create system manager
            _systemManager = new SystemManager(this);
            _systemManager.RegisterSystem("command", new CommandSystem(this));
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            _systemManager.RunSystems(argument, updateSource);
        }

        private static CommandRepository BuildCommandRepository()
        {
            var commandRepository = new CommandRepository();

            var helpCommand = new HelpCommand(commandRepository);
            commandRepository.RegisterCommand(helpCommand);

            commandRepository.RegisterCommand(new ApplyCommand());
            commandRepository.RegisterCommand(new ListActionsCommand());
            commandRepository.RegisterCommand(new ListCommonActionsCommand());
            commandRepository.RegisterCommand(new SetAntennaTextCommand());

            return commandRepository;
        }
    }
}