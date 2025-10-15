using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
        public readonly IItemContainer Container;

        private readonly IRepositoryManager _repositoryManager;
        private readonly ISystemManager _systemManager;

        public Program()
        {
            // Create Container
            Container = new ItemContainer();

            // Create config
            Container.RegisterItem(new ScriptConfig());

            // Create Event Bus
            Container.RegisterItem(new SimpleEventBus<IConnectorEvent>());

            // Create Repositories
            _repositoryManager = new RepositoryManager(Container);
            Container.RegisterItem(new DockGroupRepository());
            Container.RegisterItem(new ConnectionHistoryRepository(this));
            Container.RegisterItem(new DisplayRepository());

            // Load data
            Echo("Loading previous data...");
            _repositoryManager.LoadStorage(
                Storage,
                onSuccess: () => Echo("Previous data loaded."),
                onError: e => Echo($"Failed to load previous data: {e.Message}.\nPrevious data deleted")
            );

            // Create Services
            Container.RegisterItem(new GroupTextPanelService(this));
            
            // Create processors
            Container.RegisterItem(new TextPanelConnectionProcessor(this));
            Container.RegisterItem(new ConnectionHistoryProcessor(this));
            Container.RegisterItem(new TimerBlocConnectionProcessor(this));

            // Create systems
            _systemManager = new SystemManager(this);
            _systemManager.RegisterSystems(
                SystemGroups.Detection,
                UpdateType.Update100,
                new List<ISystem>
                {
                    new GroupDetectionSystem(this),
                    new GroupTimerBlocDetectionSystem(this),
                    new GroupTextPanelDetectionSystem(this),
                    new DisplayDetectionSystem(this)
                }
            );

            _systemManager.RegisterSystems(
                SystemGroups.Display,
                UpdateType.Update10,
                new List<ISystem>
                {
                    new HistoryDisplaySystem(this)
                }
            );
            
            _systemManager.RegisterSystems(
                SystemGroups.Runtime,
                UpdateType.Update10,
                new List<ISystem>
                {
                    new ConnectionTriggerSystem(this)
                }
            );
            
            _systemManager.RegisterSystems(
                SystemGroups.Command,
                UpdateType.Terminal | UpdateType.Trigger | UpdateType.Mod,
                new List<ISystem>
                {
                    new CommandSystem(this)
                }
            );

            Runtime.UpdateFrequency = _systemManager.GetUpdateFrequency();
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