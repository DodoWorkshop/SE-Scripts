using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        private readonly SystemManager _systemManager;

        public Program()
        {
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
    }
}