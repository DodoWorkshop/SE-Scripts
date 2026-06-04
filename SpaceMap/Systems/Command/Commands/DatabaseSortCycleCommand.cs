using System;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class DatabaseSortCycleCommand : ICommand
    {
        private readonly DatabaseViewSettings _viewSettings;
        private readonly int _sortModeCount;

        public DatabaseSortCycleCommand(Program program)
        {
            _viewSettings = program.Container.GetItem<DatabaseViewSettings>();
            _sortModeCount = Enum.GetValues(typeof(DatabaseSortMode)).Length;
        }

        public string[] Names => new[] { "db_sort_next", "dbsn" };

        public string Execute(MyCommandLine commandLine)
        {
            var next = ((int)_viewSettings.SortMode + 1) % _sortModeCount;
            _viewSettings.SortMode = (DatabaseSortMode)next;
            _viewSettings.ScrollOffset = 0;
            return $"Sort: {_viewSettings.SortMode}";
        }

        public string GetUsage()
        {
            return "db_sort_next  alias: dbsn\n" +
                   "  Cycle to the next database sort mode (Distance → Name → Age → New).\n" +
                   "  Example: db_sort_next | dbsn";
        }
    }
}
