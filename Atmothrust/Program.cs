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
        private IMyTextPanel _textPanel;
        private IMyCockpit _cockpit;

        public Program()
        {
            _cockpit = GridTerminalSystem.GetBlockWithName("Cockpit") as IMyCockpit;
            _textPanel = GridTerminalSystem.GetBlockWithName("TestTextPanel") as IMyTextPanel;
            
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            var thrust = _cockpit.MoveIndicator;
            _textPanel.WriteText($"Thrust: {thrust.X}/{thrust.Y}/{thrust.Z}");
        }
    }
}