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
                    var displayMode = DisplayMode.General;
                    if (match.Groups.Count == 2)
                    {
                        displayMode = (DisplayMode)Enum.Parse(typeof(DisplayMode), match.Groups[1].Value);
                    }

                    var panelSurface = new PanelSurface((IMyTextSurface)block, displayMode);

                    return new Panel(block, new List<PanelSurface> { panelSurface });
                }

                if (block is IMyTextSurfaceProvider)
                {
                    var provider = (IMyTextSurfaceProvider)block;
                    var match = _displayNameRegex.Match(block.CustomName);
                    if (match.Groups.Count == 1 || match.Groups[1].Value == "Auto")
                    {
                        var surfaces = new List<PanelSurface>
                        {
                            new PanelSurface(provider.GetSurface(0), DisplayMode.General)
                        };

                        if (provider.SurfaceCount > 1)
                        {
                            surfaces.Add(new PanelSurface(provider.GetSurface(1), DisplayMode.Map));
                        }

                        if (provider.SurfaceCount > 2)
                        {
                            surfaces.Add(new PanelSurface(provider.GetSurface(2), DisplayMode.Database));
                        }

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
                                var mode = (DisplayMode)Enum.Parse(typeof(DisplayMode), partSplit[1]);

                                surfaces.Add(new PanelSurface(provider.GetSurface(index), mode));
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