using System.Text;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class ApplyCommand : ICommand
    {
        private readonly IBlocMatcher _blocMatcher = new CommonBlocMatcher();

        public string[] Names { get; } =
        {
            "apply",
            "ap"
        };

        public void Execute(Program program, CommandInput input)
        {
            // Get targets
            var targets = _blocMatcher.GetMatchingBlocs(program, input.Options);

            // Applying action
            foreach (var action in input.Arguments)
            {
                if (Program.LogExecution)
                    program.Echo($"\nApplying \"{action}\" to:");

                foreach (var bloc in targets)
                {
                    if (Program.LogExecution)
                        program.Echo($"- {bloc.CustomName}");
                    bloc.ApplyAction(action);
                }
                
                program.Echo("\n<Result>--------------------");
                program.Echo("Done");
            }
        }

        public string GetUsage()
        {
            var sb = new StringBuilder();

            sb.AppendLine("< Apply Action >");
            sb.AppendLine("Commands: " + string.Join(", ", Names));
            sb.AppendLine("\nDescription: Apply provided actions to matching blocs.");
            sb.AppendLine($"\nUsage: {Names[0]} -{{optionName}}={{optionValue}} \"{{actionName}}\"");

            sb.AppendLine("\nOptions:");
            foreach (var usage in _blocMatcher.GetOptionsUsages())
            {
                sb.AppendLine($"  {usage}");
            }

            sb.AppendLine("\nExamples:");
            sb.AppendLine("ap -tag=Battery -tag=\"My awesome ship\" -grid=true OnOff_On Auto");
            sb.AppendLine(
                "-> Toggles On and Auto mode for every blocs on the current grid that contains \"Battery\" and \"My awesome ship\" texts in their names");

            return sb.ToString();
        }
    }
}