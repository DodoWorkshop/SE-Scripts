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
        // [Settings]
        public const string AutoFilledContainerTag = "[AutoFilled]";
        
        // [Variables]
        public ReferenceBinding ReferenceBinding { get; }

        private readonly SystemManager _systemManager;
        private readonly IRepositoryManager _repositoryManager;
        private readonly MyIni _ini;
        
        // [Execution]
        public Program()
        {
            _systemManager = new SystemManager(this);
            _repositoryManager = new RepositoryManager();
            _ini = new MyIni();
            
            ReferenceBinding = new ReferenceBinding(this);
            
            _systemManager.RegisterSystem(new CommandHandlerSystem());
        }

        public void Save()
        {
            _repositoryManager.SaveRepositories(_ini);

            Storage = _ini.ToString();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            _systemManager.RunSystems(argument, updateSource);
        }
    }
}