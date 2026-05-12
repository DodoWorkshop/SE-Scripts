using System;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class DatabaseSortCommand : ICommand
    {
        private readonly DatabaseViewSettings _viewSettings;

        public DatabaseSortCommand(Program program)
        {
            _viewSettings = program.Container.GetItem<DatabaseViewSettings>();
        }

        public string[] Names => new[] { "db_sort", "dbso" };

        public void Execute(MyCommandLine commandLine)
        {
            if (commandLine.ArgumentCount < 2)
                throw new Exception("Sort mode required. Available: distance, name, age, new");

            _viewSettings.SortMode = (DatabaseSortMode)Enum.Parse(typeof(DatabaseSortMode), commandLine.Argument(1), true);
            _viewSettings.ScrollOffset = 0;
        }

        public string GetUsage()
        {
            return "db_sort <mode>  alias: dbso\n" +
                   "  Sort the database view. Modes: distance, name, age, new\n" +
                   "  Example: db_sort name | dbso distance";
        }
    }
}
