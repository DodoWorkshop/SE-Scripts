using System;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class CommandSystem : IRuntimeSystem
    {
        private static string HelpMessage =>
            "Run script with \"help\" argument to get available commands";

        private readonly MyCommandLine _commandLine;
        private readonly Program _program;
        private readonly CommandReader _commandReader;
        private readonly CommandRepository _commandRepository;

        public CommandSystem(Program program)
        {
            _program = program;
            _commandLine = _program.Container.GetItem<MyCommandLine>();
            _commandReader = _program.Container.GetItem<CommandReader>();
            _commandRepository = _program.Container.GetItem<CommandRepository>();
        }

        public void Run(string argument, UpdateType updateSource)
        {
            if (string.IsNullOrEmpty(argument)) return;

            // Parse command
            _commandLine.TryParse(argument);
            if (_commandLine.Items.Count == 0)
            {
                throw new Exception($"A command should be provided. {HelpMessage}");
            }

            // Map to internal object
            var commandInput = _commandReader.ReadCommand(_commandLine);
            var command = _commandRepository.GetCommand(commandInput.Command);
            if (command == null)
            {
                throw new Exception($"No command with name '{commandInput.Command}' found. {HelpMessage}");
            }

            // Execution
            try
            {
                if (Program.LogCommandInfo)
                {
                    _program.Echo("<Command Info>--------------------");
                    _program.Echo(commandInput.ToString());
                }

                if (Program.LogExecution)
                {
                    _program.Echo("<Command Execution>---------------");
                }
                command.Execute(_program, commandInput);
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"An error occured while executing command \"{commandInput.Command}\": {e.Message}");
            }
        }
    }
}