using System.Collections.Generic;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class CommandReader
    {
        private readonly System.Text.RegularExpressions.Regex _optionRegex =
            new System.Text.RegularExpressions.Regex(@"-([a-z]+)=(?:""([^""]+)|(\S+))");

        public CommandInput ReadCommand(MyCommandLine commandLine)
        {
            var commandName = commandLine.Items[0];
            var options = new List<CommandOption>();
            var arguments = new List<string>();

            for (var i = 1; i < commandLine.Items.Count; i++)
            {
                var str = commandLine.Items[i];

                // Try to match option
                var match = _optionRegex.Match(str);
                if (match.Success)
                {
                    options.Add(
                        new CommandOption(
                            match.Groups[1].Value,
                            match.Groups[2].Value + match.Groups[3].Value
                        )
                    );
                }

                // Otherwise, it's an argument
                else
                {
                    arguments.Add(str);
                }
            }

            return new CommandInput(
                commandName,
                options.ToArray(),
                arguments.ToArray()
            );
        }
    }
}