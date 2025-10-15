using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class CommandSystem : IRuntimeSystem
    {
        private readonly Program _program;
        private readonly DisplayRepository _displayRepository;
        private readonly ConnectionHistoryRepository _connectionHistoryRepository;

        public CommandSystem(Program program)
        {
            _program = program;
            _displayRepository = _program.Container.GetItem<DisplayRepository>();
            _connectionHistoryRepository = _program.Container.GetItem<ConnectionHistoryRepository>();
        }

        public void Run(string argument, UpdateType updateSource)
        {
            if (string.IsNullOrEmpty(argument))
                return;

            switch (argument)
            {
                case "hnext":
                    _displayRepository.HistoryPage++;
                    break;
                case "hprev":
                    _displayRepository.HistoryPage--;
                    if (_displayRepository.HistoryPage < 0)
                    {
                        _displayRepository.HistoryPage = 0;
                    }

                    break;
                case "hfirst":
                    _displayRepository.HistoryPage = 0;
                    break;
                case "hreset":
                    _connectionHistoryRepository.Clear();
                    break;
            }
        }
    }
}