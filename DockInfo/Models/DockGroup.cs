using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace IngameScript
{
    public class DockGroup
    {
        public string Name { get; set; }

        public IMyShipConnector Connector { get; set; }

        public HashSet<IMyTextPanel> TextPanels { get; } = new HashSet<IMyTextPanel>();
        
        public HashSet<IMyTimerBlock> TimerBlocks { get; } = new HashSet<IMyTimerBlock>();

        public bool IsConnected { get; set; }
    }
}