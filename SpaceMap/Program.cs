using System;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public partial class Program : MyGridProgram
    {
    #region mdk preserve

    // ===================== SETTINGS ===================== //

    // [ Mode ]
    // How this programmable block operates.
    //   Ship       - Detects entities via camera raycast, maintains a local database.
    //   Server     - Receives and merges databases broadcast by nearby ships.
    //   ServerShip - Both detects and acts as a server.
    public const string Mode = "Ship";

    // [ Sync ]
    // IGC channel used for ship↔server database synchronisation.
    public const string SyncChannel = "SM_SYNC";
    // Interval (in Update10 ticks, ~6/s) between ship database broadcasts. 360 ~ 60s.
    public const int SyncBroadcastInterval = 360;

    // [ Detection ]
    // Interval (in ticks) between block discovery scans. Lower = more responsive, higher = better perf.
    public const int SearchBlockInterval = 100;
    // Default camera raycast range in meters (changeable at runtime with: detect_range <value>).
    public const uint DefaultDetectionDistance = 10000;
    // Default map display radius in meters (changeable at runtime with: map_scale <value>).
    public const uint DefaultMapScale = 5000;

    // [ Database ]
    // Seconds after which a freshly detected entry is no longer marked as new ([!] on displays).
    public const long NewEntryThresholdSeconds = 60;

    // [ Map display ]
    // Strength of the isometric depth offset on the Map view (0 = flat, 1 = extreme).
    public const float MapPerspectiveStrength = 0.2f;

    // [ Map3D display ]
    // Vertical compression of the horizontal-plane ellipse (0 = flat line, 1 = circle).
    public const float Map3DEllipseRatio = 0.4f;
    // Scale of the altitude axis relative to the horizontal scale (higher = taller depth lines).
    public const float Map3DDepthScale = 0.65f;

    // ===================== END SETTINGS ===================== //
    #endregion

        public readonly IItemContainer Container;

        private readonly ISystemManager _systemManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly CommandFeedback _commandFeedback;
        private string _errorLog = null;
        private int _spinnerState;
        private static readonly char[] Spinner = { '|', '/', '-', '\\' };

        public Program()
        {
            Container = new ItemContainer();

            _repositoryManager = new RepositoryManager(Container);
            _systemManager = new SystemManager(this);

            BuildRepositories();
            BuildCommunicationLayer();
            BuildSystems();

            _commandFeedback = Container.GetItem<CommandFeedback>();

            Runtime.UpdateFrequency = UpdateFrequency.Update100 | UpdateFrequency.Update10;
        }

        private void BuildRepositories()
        {
            Container.RegisterItem<IMapEntryRepository>(new MapEntryRepository());
            Container.RegisterItem<IUserSettingsRepository>(new UserSettingsRepository());
            Container.RegisterItem<IDetectionDataRepository>(new DetectionDataRepository());
            Container.RegisterItem<DatabaseViewSettings>(new DatabaseViewSettings());
            Container.RegisterItem<DisplayCycleSettings>(new DisplayCycleSettings());
            Container.RegisterItem<CommandFeedback>(new CommandFeedback());

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

            Container.RegisterItem<SyncStats>(new SyncStats(scriptMode));

            switch (scriptMode)
            {
                case ScriptMode.Ship:
                    _systemManager.RegisterSystem(SystemGroups.Logic, new ShipSystem(this));
                    _systemManager.RegisterSystem(SystemGroups.Logic, new LocalDatabaseSystem(this));
                    _systemManager.RegisterSystem(SystemGroups.Logic, new ShipSyncSystem(this));
                    break;
                case ScriptMode.Server:
                    _systemManager.RegisterSystem(SystemGroups.Logic, new ServerDatabaseSystem(this));
                    break;
                case ScriptMode.ServerShip:
                    _systemManager.RegisterSystem(SystemGroups.Logic, new ShipSystem(this));
                    _systemManager.RegisterSystem(SystemGroups.Logic, new LocalDatabaseSystem(this));
                    _systemManager.RegisterSystem(SystemGroups.Logic, new ShipSyncSystem(this));
                    _systemManager.RegisterSystem(SystemGroups.Logic, new ServerDatabaseSystem(this));
                    break;
            }

            var ihmSystem = new IhmSystem(this);
            Container.RegisterItem<IhmSystem>(ihmSystem);
            _systemManager.RegisterSystem(SystemGroups.Render, ihmSystem);
            _systemManager.RegisterSystem(SystemGroups.Logic, new BlocDetectionTimer(this));

            var commandSystem = new CommandSystem(this);
            Container.RegisterItem<CommandSystem>(commandSystem);
            _systemManager.RegisterSystem(SystemGroups.Command, commandSystem);

            // Set Update types
            _systemManager.SetGroupUpdateFrequency(SystemGroups.Logic, UpdateFrequency.Update10);
            _systemManager.SetGroupUpdateFrequency(SystemGroups.Render, UpdateFrequency.Update10);
        }

        public void Save()
        {
            if (_errorLog != null)
            {
                Me.CustomData = _errorLog;
                return;
            }
            var data = _repositoryManager.SaveToString();
            Storage = data;
            Me.CustomData = data;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            _spinnerState = (_spinnerState + 1) % Spinner.Length;
            _commandFeedback.Tick();

            try
            {
                _systemManager.RunSystems(argument, updateSource);
            }
            catch (Exception e)
            {
                _errorLog = BuildErrorLog(e);
                Me.CustomData = _errorLog;
                Echo(_errorLog);
                return;
            }

            var echo = "Running " + Spinner[_spinnerState];
            if (_commandFeedback.HasMessage)
                echo += "\n\n" + (_commandFeedback.IsError ? "ERR - " : "OK - ") + _commandFeedback.Message;
            Echo(echo);
        }

        private string BuildErrorLog(Exception e)
        {
            var log = "=== SpaceMap Error ===\n";
            var current = e;
            while (current != null)
            {
                log += "[" + current.GetType().Name + "] " + current.Message + "\n";
                if (current.StackTrace != null)
                    log += current.StackTrace + "\n";
                current = current.InnerException;
                if (current != null)
                    log += "--- Inner Exception ---\n";
            }
            return log;
        }
    }
}