using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class IhmBindings
    {
        public IEnumerable<Panel> Panels => _panelsPerBlockId.Values;

        private readonly Dictionary<long, Panel> _panelsPerBlockId = new Dictionary<long, Panel>();

        private readonly MyRegex _displayNameRegex =
            new MyRegex(Settings.DisplayNameRegex);

        public void SearchScreens(Program program)
        {
            var localGrid = program.Me.CubeGrid;
            var blocks = new List<IMyTerminalBlock>();
            program.GridTerminalSystem.GetBlocksOfType(
                blocks,
                block => block.CubeGrid.Equals(localGrid)
                         && (block is IMyTextSurface || block is IMyTextSurfaceProvider)
                         && _displayNameRegex.IsMatch(block.CustomName)
                         // Check if the block is present but its name has changed
                         && (
                             !_panelsPerBlockId.ContainsKey(block.EntityId)
                             || !_panelsPerBlockId[block.EntityId].InitialName.Equals(block.CustomName)
                         )
            );

            var newPanels = blocks.Select(block =>
            {
                if (block is IMyTextSurface)
                {
                    var match = _displayNameRegex.Match(block.CustomName);
                    var displayMode = DisplayMode.Map;
                    if (match.Groups.Count >= 2 && !string.IsNullOrEmpty(match.Groups[1].Value))
                    {
                        try
                        {
                            displayMode = (DisplayMode)Enum.Parse(typeof(DisplayMode), match.Groups[1].Value);
                        }
                        catch { }
                    }

                    var panelSurface = new PanelSurface((IMyTextSurface)block, displayMode);

                    return new Panel(block, new List<PanelSurface> { panelSurface });
                }

                if (block is IMyTextSurfaceProvider)
                {
                    var provider = (IMyTextSurfaceProvider)block;
                    var match = _displayNameRegex.Match(block.CustomName);
                    if (match.Groups.Count <= 1 || string.IsNullOrEmpty(match.Groups[1].Value) || match.Groups[1].Value == "Auto")
                    {
                        var surfaces = new List<PanelSurface>();

                        if (provider.SurfaceCount > 0)
                            surfaces.Add(new PanelSurface(provider.GetSurface(0), DisplayMode.Map));
                        if (provider.SurfaceCount > 1)
                            surfaces.Add(new PanelSurface(provider.GetSurface(1), DisplayMode.Map3D));
                        if (provider.SurfaceCount > 2)
                            surfaces.Add(new PanelSurface(provider.GetSurface(2), DisplayMode.Database));

                        return new Panel(block, surfaces);
                    }
                    else
                    {
                        var surfaces = new List<PanelSurface>();
                        var split = match.Groups[1].Value.Split(';');
                        foreach (var part in split)
                        {
                            var partSplit = part.Split('-');
                            if (partSplit.Length == 2)
                            {
                                var index = int.Parse(partSplit[0]);
                                DisplayMode mode;
                        try { mode = (DisplayMode)Enum.Parse(typeof(DisplayMode), partSplit[1]); }
                        catch { continue; }
                                var surf = provider.GetSurface(index);
                                if (surf != null)
                                    surfaces.Add(new PanelSurface(surf, mode));
                            }
                        }

                        return new Panel(block, surfaces);
                    }
                }

                throw new Exception("Not handled type: " + block.GetType().Name);
            });

            foreach (var panel in newPanels)
            {
                _panelsPerBlockId[panel.Block.EntityId] = panel;
            }
        }
    }
}