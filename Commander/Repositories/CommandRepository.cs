using System.Collections.Generic;
using System.Linq;

namespace IngameScript
{
    public class CommandRepository
    {
        private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

        public void RegisterCommand(ICommand command)
        {
            foreach (var commandName in command.Names)
            {
                _commands.Add(commandName, command);
            }
        }

        public ICommand GetCommand(string name)
        {
            return _commands.GetValueOrDefault(name);
        }

        public ICommand[] GetAllCommands() => _commands.Values.ToArray();
    }
}