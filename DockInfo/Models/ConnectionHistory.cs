using System;

namespace IngameScript
{
    public struct ConnectionHistory
    {
        public long GridId { get; }

        public string GridName { get; }

        public HistoryType HistoryType { get; }

        public DateTime Timestamp { get; }

        public string DockGroupName { get; }

        public ConnectionHistory(long gridId, string gridName, HistoryType type, DateTime timestamp,
            string dockGroupName)
        {
            GridId = gridId;
            GridName = gridName;
            HistoryType = type;
            Timestamp = timestamp;
            DockGroupName = dockGroupName;
        }

        public string Serialize()
        {
            return $"{GridId},{GridName},{HistoryType},{Timestamp},{DockGroupName}";
        }

        public static ConnectionHistory Deserialize(string line)
        {
            var split = line.Split(',');
            return new ConnectionHistory(
                long.Parse(split[0]),
                split[1],
                (HistoryType)Enum.Parse(typeof(HistoryType), split[2]),
                DateTime.Parse(split[3]),
                split[4]
            );
        }
    }
}