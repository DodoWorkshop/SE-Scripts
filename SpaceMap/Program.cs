using System;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
    #region mdk preserve

    // ------------------ [ SETTINGS ] ------------------ //

    // [ Common ]

    // Indicates the way the script will be used.
    // Possible value are:
    //   - Ship: will detect entities and will have a local database that can be synchronized with a database server
    //   - Server: won't detect entities but will store data from ships databases
    //   - ServerShip: will detect entities and will act as a server
    public const string Mode = "Ship";

    // The interval (in ticks) between each attempt to detect grid blocks. Low value will impact performances.
    public const int SearchBlockInterval = 100;


    // --------- [ Don't touch anything below ]--------- //
    #endregion

        public readonly IItemContainer Container;

        private readonly ISystemManager _systemManager;
        private readonly IRepositoryManager _repositoryManager;

        public Program()
        {
            Container = new ItemContainer();

            _repositoryManager = new RepositoryManager(Container);
            _systemManager = new SystemManager(this);

            BuildRepositories();
            BuildCommunicationLayer();
            BuildSystems();

            Runtime.UpdateFrequency = UpdateFrequency.Update100 | UpdateFrequency.Update10;
        }

        private void BuildRepositories()
        {
            Container.RegisterItem<IMapEntryRepository>(new MapEntryRepository());
            Container.RegisterItem<IUserSettingsRepository>(new UserSettingsRepository());
            Container.RegisterItem<IDetectionDataRepository>(new DetectionDataRepository());

            _repositoryManager.LoadStorage(Storage);
        }

        private void BuildCommunicationLayer()
        {
            var bus = new SimpleEventBus<ISpaceMapEvent>();
            Container.RegisterItem<IEventSink<ISpaceMapEvent>>(bus);
            Container.RegisterItem<IEventStream<ISpaceMapEvent>>(bus);
        }

        private void BuildSystems()
        {
            ScriptMode scriptMode;
            try
            {
                scriptMode = (ScriptMode)Enum.Parse(typeof(ScriptMode), Mode, true);
            }
            catch (Exception)
            {
                throw new Exception("Invalid mode provided. Available values are: " +
                                    string.Join(", ", Enum.GetNames(typeof(ScriptMode))));
            }

            switch (scriptMode)
            {
                case ScriptMode.Ship:
                    _systemManager.RegisterSystem(SystemGroups.Logic, new ShipSystem(this));
                    _systemManager.RegisterSystem(SystemGroups.Logic, new LocalDatabaseSystem(this));
                    break;
                case ScriptMode.Server:
                    _systemManager.RegisterSystem(SystemGroups.Logic, new ServerDatabaseSystem());
                    break;
                case ScriptMode.ServerShip:
                    _systemManager.RegisterSystem(SystemGroups.Logic, new ShipSystem(this));
                    _systemManager.RegisterSystem(SystemGroups.Logic, new ServerDatabaseSystem());
                    break;
            }

            _systemManager.RegisterSystem(SystemGroups.Render, new IhmSystem(this));
            _systemManager.RegisterSystem(SystemGroups.Logic, new BlocDetectionTimer(this));
            _systemManager.RegisterSystem(SystemGroups.Command, new CommandSystem(this));

            // Set Update types
            _systemManager.SetGroupUpdateFrequency(SystemGroups.Logic, UpdateFrequency.Update10);
            _systemManager.SetGroupUpdateFrequency(SystemGroups.Render, UpdateFrequency.Update100);
        }

        public void Save()
        {
            var data = _repositoryManager.SaveToString();
            Storage = data;
            Me.CustomData = data;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            _systemManager.RunSystems(argument, updateSource);
        }
    }
}