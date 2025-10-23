using System;
using System.Linq;
using System.Text;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class CommandSystem : IRuntimeSystem
    {
        private const string HelpMessage = "Run script with \"help\" argument to get available commands";


        private readonly MyCommandLine _commandLine;
        private readonly Program _program;
        private readonly ICommand[] _commands;

        public CommandSystem(Program program)
        {
            _program = program;
            _commandLine = new MyCommandLine();
            _commands = new ICommand[]
            {
                new SetMapScaleCommand(_program)
            };
        }

        public void Run(string argument, UpdateType updateSource)
        {
            if (updateSource != UpdateType.Terminal && string.IsNullOrEmpty(argument)) return;

            _commandLine.TryParse(argument);
            if (_commandLine.Items.Count == 0)
            {
                throw new Exception($"A command should be provided. {HelpMessage}");
            }

            var commandName = _commandLine.Items[0];

            // Handle help case
            if (commandName.Equals("help") || commandName.Equals("h"))
            {
                PrintUsage();
                return;
            }

            var command = _commands.FirstOrDefault(c =>
                c.Names.Any(n => string.Equals(n, commandName, StringComparison.OrdinalIgnoreCase))
            );

            if (command == null)
            {
                throw new Exception($"No command with name '{commandName}' found. {HelpMessage}");
            }

            try
            {
                command.Execute(_commandLine);
            }
            catch (Exception e)
            {
                throw new Exception($"An error occured while executing command '{commandName}': {e.Message}");
            }
        }

        private void PrintUsage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("--- Space Map Script ---");
            sb.AppendLine("TODO\n");

            sb.AppendLine("----------");
            sb.AppendLine("Usage: [commandName] [arguments]*\n");
            sb.AppendLine("Available commands:\n");

            foreach (var command in _commands)
            {
                sb.AppendLine(command.GetUsage());
            }

            _program.Echo(sb.ToString());
        }
    }
}