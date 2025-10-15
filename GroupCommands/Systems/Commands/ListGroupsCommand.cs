using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class ListGroupsCommand : ICommand
    {
        public string[] Names { get; } =
        {
            "list-groups",
            "lg"
        };
        
        public void Execute(Program program, MyCommandLine commandLine)
        {
            var grid = program.Me.CubeGrid;
            var groups = new List<IMyBlockGroup>();
            program.GridTerminalSystem.GetBlockGroups(groups, g =>
            {
                var blocks = new List<IMyTerminalBlock>();
                g.GetBlocks(blocks);

                return blocks.Any(b => b.CubeGrid.Equals(grid));
            });

            var sb = new StringBuilder();
            sb.AppendLine("Available groups:");
            foreach (var group in groups)
            {
                sb.AppendLine($"- {group.Name}");
            }

            program.Echo(sb.ToString());
        }
        
        public string GetUsage()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("< List Groups >");
            sb.AppendLine("Commands: " + string.Join(", ", Names));
            sb.AppendLine("Description: List every available groups. It will ignore groups from other grids.");
            sb.AppendLine($"Usage: {Names[0]}");
            
            return sb.ToString();
        }
    }
}