using System;
using System.Linq;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class CommandSystem : IRuntimeSystem
    {
        private readonly MyCommandLine _commandLine;
        private readonly Program _program;
        private readonly ICommand[] _commands;
        private readonly CommandFeedback _feedback;

        public ICommand[] Commands => _commands;

        public CommandSystem(Program program)
        {
            _program = program;
            _commandLine = new MyCommandLine();
            _feedback = program.Container.GetItem<CommandFeedback>();
            _commands = new ICommand[]
            {
                new SetMapScaleCommand(_program),
                new SetDetectionRangeCommand(_program),
                new RenameEntryCommand(_program),
                new DatabaseScrollCommand(_program),
                new DatabaseSortCommand(_program),
                new DatabaseSortCycleCommand(_program),
                new DispNextCommand(_program)
            };
        }

        public void Run(string argument, UpdateType updateSource)
        {
            if (string.IsNullOrEmpty(argument)) return;

            _commandLine.TryParse(argument);
            if (_commandLine.Items.Count == 0) return;

            var commandName = _commandLine.Items[0];

            var command = _commands.FirstOrDefault(c =>
                c.Names.Any(n => string.Equals(n, commandName, StringComparison.OrdinalIgnoreCase))
            );

            if (command == null)
            {
                _feedback.SetError($"Unknown command '{commandName}'");
                return;
            }

            try
            {
                var message = command.Execute(_commandLine);
                if (message != null)
                    _feedback.SetSuccess(message);
            }
            catch (Exception e)
            {
                _feedback.SetError(e.Message);
            }
        }
    }
}
