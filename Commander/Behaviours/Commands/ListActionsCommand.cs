using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.ModAPI.Interfaces;

namespace IngameScript
{
    public class ListActionsCommand : ICommand
    {
        private readonly IBlocMatcher _blocMatcher = new CommonBlocMatcher();

        public string[] Names { get; } =
        {
            "list-actions",
            "la"
        };

        public void Execute(Program program, CommandInput input)
        {
            // Get targets
            var targets = _blocMatcher.GetMatchingBlocs(program, input.Options);

            // Get actions
            var allActions = targets
                .SelectMany(target =>
                {
                    var actions = new List<ITerminalAction>();
                    target.GetActions(actions);
                    return actions;
                })
                .Distinct();

            // Display
            var sb = new StringBuilder();
            sb.AppendLine("Available actions:");
            foreach (var action in allActions)
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
            sb.AppendLine("\nDescription: List every possible actions for matching blocs (common and not common).");
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