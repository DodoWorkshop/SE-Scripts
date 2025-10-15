using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.ModAPI.Interfaces;

namespace IngameScript
{
    public class ListCommonActionsCommand : ICommand
    {
        private readonly IBlocMatcher _blocMatcher = new CommonBlocMatcher();

        public string[] Names { get; } =
        {
            "list-common-actions",
            "lca"
        };

        public void Execute(Program program, CommandInput input)
        {
            // Get targets
            var targets = _blocMatcher.GetMatchingBlocs(program, input.Options);

            // Get actions
            List<ITerminalAction> allActions = null;
            foreach (var target in targets)
            {
                // Create first actions
                if (allActions == null)
                {
                    allActions = new List<ITerminalAction>();
                    target.GetActions(allActions);
                }
                // Handle others
                else
                {
                    var actions = new List<ITerminalAction>();
                    target.GetActions(actions);
                    foreach (var action in actions.Where(action => !allActions.Contains(action)))
                    {
                        allActions.Remove(action);
                    }
                }
            }

            // Display
            if (allActions == null || allActions.Count == 0)
            {
                program.Echo("No common actions found between those matching targets:");
                foreach (var target in targets)
                {
                    program.Echo($"- {target.CustomName}");
                }
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("Available common actions:");
                foreach (var action in allActions)
                {
                    sb.AppendLine($"- {action.Id}: {action.Name}");
                }

                program.Echo(sb.ToString());
            }
        }

        public string GetUsage()
        {
            var sb = new StringBuilder();

            sb.AppendLine("< List Actions >");
            sb.AppendLine("Commands: " + string.Join(", ", Names));
            sb.AppendLine("\nDescription: List every common actions for matching blocs.");
            sb.AppendLine($"\nUsage: {Names[0]} -{{optionName}}={{optionValue}}");

            sb.AppendLine("\nOptions:");
            foreach (var usage in _blocMatcher.GetOptionsUsages())
            {
                sb.AppendLine($"  {usage}");
            }
            
            return sb.ToString();
        }
    }
}