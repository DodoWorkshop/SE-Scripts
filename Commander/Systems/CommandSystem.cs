using System;
using System.Linq;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class CommandSystem : IRuntimeSystem
    {
        private string HelpMessage =>
            $"Run script with \"{_helpCommand.Names.First()}\" argument to get available commands";

        private readonly MyCommandLine _commandLine;
        private readonly Program _program;
        private readonly CommandReader _commandReader;
        private readonly CommandRepository _commandRepository;

        private ICommand _helpCommand;

        public CommandSystem(Program program)
        {
            _program = program;
            _commandLine = new MyCommandLine();
            _commandReader = new CommandReader();
            _commandRepository = new CommandRepository();

            RegisterCommands();
        }

        public UpdateType HandledUpdateType => UpdateType.Terminal | UpdateType.Trigger | UpdateType.Mod;

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
                _program.Echo("[Command Info]--------------------");
                _program.Echo(commandInput.ToString());
                _program.Echo("[Command Execution]---------------");
                command.Execute(_program, commandInput);
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"An error occured while executing command \"{commandInput.Command}\": {e.Message}");
            }
        }

        private void RegisterCommands()
        {
            _helpCommand = new HelpCommand(_commandRepository);
            _commandRepository.RegisterCommand(_helpCommand);
            
            _commandRepository.RegisterCommand(new ApplyCommand());
            _commandRepository.RegisterCommand(new ListActionsCommand());
            _commandRepository.RegisterCommand(new ListCommonActionsCommand());
            _commandRepository.RegisterCommand(new SetAntennaTextCommand());
        }
    }
}