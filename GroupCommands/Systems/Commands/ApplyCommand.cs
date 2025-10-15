using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class ApplyCommand : ICommand
    {
        public string[] Names { get; } =
        {
            "apply",
            "ap"
        };
        
        public void Execute(Program program, MyCommandLine commandLine)
        {
            if (commandLine.Items.Count < 2)
            {
                throw new Exception("A group name is required.");
            }
            var groupName = commandLine.Items[1];
            
            if (commandLine.Items.Count < 3)
            {
                throw new Exception("An action id is required.");
            }
            var action = commandLine.Items[2];
            
            
            var group = program.GridTerminalSystem.GetBlockGroupWithName(groupName);
            if (group == null)
            {
                throw new Exception($"No group named '{groupName}' found.");
            }

            var grid = program.Me.CubeGrid;
            var blocks = new List<IMyTerminalBlock>();
            group.GetBlocks(blocks);
            
            blocks = blocks.Where(b => b.CubeGrid.Equals(grid))
                .ToList();

            if (blocks.Count == 0)
            {
                throw new Exception($"A group named '{groupName}' has been found but none of its blocks belongs to '{grid.CustomName}'");
            }

            foreach (var block in blocks)
            {
                block.ApplyAction(action);
            }
        }

        public string GetUsage()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("< Apply Action >");
            sb.AppendLine("Commands: " + string.Join(", ", Names));
            sb.AppendLine("Description: Apply the provided action to the block in the provided group and belonging to the current grid.");
            sb.AppendLine($"Usage: {Names[0]} \"{{groupName}}\" \"{{actionName}}\"");
            
            return sb.ToString();
        }
    }
}