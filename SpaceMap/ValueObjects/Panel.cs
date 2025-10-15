using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public struct Panel
    {
        public readonly IMyTerminalBlock Block;

        public readonly string InitialName;

        public readonly List<PanelSurface> Surfaces;

        public Panel(IMyTerminalBlock block, List<PanelSurface> surfaces)
        {
            Block = block;
            Surfaces = surfaces;
            InitialName = block.CustomName;
        }
    }
}