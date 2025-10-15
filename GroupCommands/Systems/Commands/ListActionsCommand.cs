using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class ListActionsCommand : ICommand
    {
        public string[] Names { get; } =
        {
            "list-actions",
            "la"
        };

        public void Execute(Program program, MyCommandLine commandLine)
        {
            if (commandLine.Items.Count < 2)
            {
                throw new Exception("An bloc name has to be provided");
            }

            var itemName = commandLine.Items[1];

            var blocs = new List<IMyTerminalBlock>();
            program.GridTerminalSystem.SearchBlocksOfName(itemName, blocs);

            var grid = program.Me.CubeGrid;

            var result = blocs.FirstOrDefault(bloc => bloc.CubeGrid.Equals(grid));
            if (result == null)
            {
                throw new Exception($"No bloc named \"{itemName}\" found on grid \"{grid.CustomName}\"");
            }

            var actions = new List<ITerminalAction>();
            result.GetActions(actions);

            var sb = new StringBuilder();
            sb.AppendLine("Available actions:");
            foreach (var action in actions)
            {
                sb.AppendLine($"- {action.Id}: {action.Name}");
            }

            program.Echo(sb.ToString());
        }

        public string GetUsage()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("< List Actions >");
            sb.AppendLine("Commands: " + string.Join(", ", Names));
            sb.AppendLine("Description: List every possible action for the bloc with the provided name");
            sb.AppendLine($"Usage: {Names[0]} \"{{blocName}}\"");
            
            return sb.ToString();
        }
    }
}