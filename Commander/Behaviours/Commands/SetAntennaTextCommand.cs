using System;
using System.Text;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class SetAntennaTextCommand : ICommand
    {
        private readonly IBlocMatcher _blocMatcher = new CommonBlocMatcher();

        public string[] Names { get; } =
        {
            "set-antenna-text",
            "sat"
        };

        public void Execute(Program program, CommandInput input)
        {
            // Get message
            if (input.Arguments.Length == 0)
            {
                throw new Exception("A message has to be provided an argument.");
            }

            var message = input.Arguments[0];

            // Get targets
            var targets = _blocMatcher.GetMatchingBlocs<IMyRadioAntenna>(program, input.Options);

            // Applying action
            if (Program.LogExecution)
                program.Echo("\nSetting message to:");

            foreach (var bloc in targets)
            {
                if (Program.LogExecution)
                    program.Echo($"- {bloc.CustomName}");
                bloc.HudText = message;
            }
            
            program.Echo("\n<Result>--------------------");
            program.Echo("Done");
        }

        public string GetUsage()
        {
            var sb = new StringBuilder();

            sb.AppendLine("< Set Antenna Text >");
            sb.AppendLine("Commands: " + string.Join(", ", Names));
            sb.AppendLine("\nDescription: Set HUD text to every matching antennas.");
            sb.AppendLine($"\nUsage: {Names[0]} -{{optionName}}={{optionValue}} \"{{message}}\"");

            sb.AppendLine("\nOptions:");
            foreach (var usage in _blocMatcher.GetOptionsUsages())
            {
                sb.AppendLine($"  {usage}");
            }

            return sb.ToString();
        }
    }
}