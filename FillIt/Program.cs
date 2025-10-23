using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        // [Settings]
        public const string AutoFilledContainerTag = "[AutoFilled]";

        // [Variables]
        public readonly IItemContainer Container;

        private readonly ISystemManager _systemManager;
        private readonly IRepositoryManager _repositoryManager;

        // [Execution]
        public Program()
        {
            Container = new ItemContainer();

            _systemManager = new SystemManager(this);
            _repositoryManager = new RepositoryManager(Container);

            Container.RegisterItem(new ReferenceBinding(this));

            _systemManager.RegisterSystem("command", new CommandHandlerSystem());
        }

        public void Save()
        {
            Storage = _repositoryManager.SaveToString();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            _systemManager.RunSystems(argument, updateSource);
        }
    }
}