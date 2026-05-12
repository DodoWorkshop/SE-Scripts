using System;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class DatabaseScrollCommand : ICommand
    {
        private readonly DatabaseViewSettings _viewSettings;
        private readonly IMapEntryRepository _mapEntryRepository;

        private const int PageStep = 5;

        public DatabaseScrollCommand(Program program)
        {
            _viewSettings = program.Container.GetItem<DatabaseViewSettings>();
            _mapEntryRepository = program.Container.GetItem<IMapEntryRepository>();
        }

        public string[] Names => new[] { "db_scroll", "dbs" };

        public void Execute(MyCommandLine commandLine)
        {
            var arg = commandLine.ArgumentCount > 1 ? commandLine.Argument(1) : "down";

            int delta;
            if (string.Equals(arg, "up", StringComparison.OrdinalIgnoreCase))
                delta = -PageStep;
            else if (string.Equals(arg, "down", StringComparison.OrdinalIgnoreCase))
                delta = PageStep;
            else if (!int.TryParse(arg, out delta))
                throw new Exception($"Expected 'up', 'down', or an integer, got '{arg}'");

            var total = _mapEntryRepository.GetAll<IMapEntry>().Count;
            _viewSettings.ScrollOffset = Math.Max(0, Math.Min(_viewSettings.ScrollOffset + delta, Math.Max(0, total - 1)));
        }

        public string GetUsage()
        {
            return "db_scroll (up|down|<n>)  alias: dbs\n" +
                   "  Scroll the database view. up/down moves by 5, or provide a number.\n" +
                   "  Example: db_scroll down | dbs -3";
        }
    }
}
