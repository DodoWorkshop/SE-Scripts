using System.Text;

namespace IngameScript
{
    public struct CommandInput
    {
        public CommandInput(string command, CommandOption[] options, string[] arguments)
        {
            Command = command;
            Options = options;
            Arguments = arguments;
        }

        public string Command { get; }

        public CommandOption[] Options { get; }

        public string[] Arguments { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Command name: {Command}");
            sb.AppendLine("\nOptions:");
            foreach (var option in Options)
            {
                sb.AppendLine($"- {option.Name}: {option.Value}");
            }

            sb.AppendLine("\nArguments:");
            foreach (var argument in Arguments)
            {
                sb.AppendLine($"- {argument}");
            }

            return sb.ToString();
        }
    }
}