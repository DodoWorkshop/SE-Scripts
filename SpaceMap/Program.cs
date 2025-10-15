using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

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
        
        
        private readonly ISystemManager _systemManager;
        public readonly IRepositoryManager RepositoryManager;
        public readonly IStoreManager StoreManager;
        public readonly IEventBus<ISpaceMapEvent> EventBus;
        private readonly MyIni _storageIni;

        public Program()
        {
            _systemManager = new SystemManager(this);
            RepositoryManager = new RepositoryManager();
            StoreManager = new StoreManager();
            EventBus = new SimpleEventBus<ISpaceMapEvent>();
            _storageIni = new MyIni();

            BuildRepositories();
            BuildStores();
            BuildSystems();

            Load();

            Runtime.UpdateFrequency = UpdateFrequency.Update100 | UpdateFrequency.Update10;
        }

        private void BuildRepositories()
        {
            RepositoryManager.RegisterRepository<IMapEntryRepository>(new MapEntryRepository());
            RepositoryManager.RegisterRepository<IUserSettingsRepository>(new UserSettingsRepository());
        }

        private void BuildStores()
        {
            StoreManager.RegisterStore<IDetectionDataStore>(new DetectionDataStore());
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
                    _systemManager.RegisterSystem(new ShipSystem(this));
                    _systemManager.RegisterSystem(new LocalDatabaseSystem(this));
                    break;
                case ScriptMode.Server:
                    _systemManager.RegisterSystem(new ServerDatabaseSystem());
                    break;
                case ScriptMode.ServerShip:
                    _systemManager.RegisterSystem(new ShipSystem(this));
                    _systemManager.RegisterSystem(new ServerDatabaseSystem());
                    break;
            }

            _systemManager.RegisterSystem(new IhmSystem(this));
            _systemManager.RegisterSystem(new BlocDetectionTimer(this));
            _systemManager.RegisterSystem(new CommandSystem(this));
            
            // TODO: fine tune groups and update types
            _systemManager.SetGroupUpdateTypes("default", UpdateType.Update10);
        }

        public void Save()
        {
            _storageIni.Clear();
            RepositoryManager.SaveRepositories(_storageIni);

            var data = _storageIni.ToString();
            Storage = data;
            Me.CustomData = data;
        }

        private void Load()
        {
            if (!string.IsNullOrEmpty(Storage))
            {
                if (!_storageIni.TryParse(Storage))
                {
                    Storage = "";
                    throw new Exception("Failed to load data");
                }

                RepositoryManager.LoadRepositories(_storageIni);
            }
        }

        public void Main(string argument, UpdateType updateSource)
        {
            _systemManager.RunSystems(argument, updateSource);
        }
    }
}