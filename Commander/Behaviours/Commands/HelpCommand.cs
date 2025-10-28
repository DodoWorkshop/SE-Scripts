using System.Text;

namespace IngameScript
{
    public class HelpCommand : ICommand
    {
        private readonly CommandRepository _commandRepository;

        public HelpCommand(CommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }


        public string[] Names { get; } =
        {
            "help",
            "h"
        };

        public void Execute(Program program, CommandInput input)
        {
            var sb = new StringBuilder();
            sb.AppendLine("--- Commander Script ---");
            sb.AppendLine(
                "Use this script to send commands to block or groups with specific restriction and arguments\n");

            sb.AppendLine("----------");
            sb.AppendLine("Usage: {commandName} {-options}* {arguments}*\n");
            sb.AppendLine("Available commands:\n");

            foreach (var command in _commandRepository.GetAllCommands())
            {
                sb.AppendLine(command.GetUsage());
            }

            program.Echo(sb.ToString());
        }

        public string GetUsage()
        {
            var sb = new StringBuilder();

            sb.AppendLine("< Help >");
            sb.AppendLine("Commands: " + string.Join(", ", Names));
            sb.AppendLine("\nDescription: Print help");
            sb.AppendLine($"\nUsage: {Names[0]}");

            return sb.ToString();
        }
    }
}