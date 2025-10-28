using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class ConnectionHistoryRepository : ISavableRepository
    {
        private readonly List<ConnectionHistory> _data = new List<ConnectionHistory>();
        private readonly int _maxHistory;
        private const string SaveKey = "history_repo";

        public ConnectionHistoryRepository(Program program)
        {
            _maxHistory = ScriptConfig.HistoryLength;
        }

        public void Register(ConnectionHistory connectionHistory)
        {
            _data.Insert(0, connectionHistory);
            if (_data.Count > _maxHistory)
            {
                _data.RemoveRange(_maxHistory, _data.Count - _maxHistory);
            }
        }

        public ConnectionHistory[] GetPage(int amount, int page)
        {
            var offset = page * amount;
            return _data.GetRange(
                    offset,
                    offset + amount >= _data.Count ? _data.Count - offset : amount
                )
                .ToArray();
        }

        public ConnectionHistory? GetLastLanding(string groupName)
        {
            return _data.FirstOrDefault(history => history.DockGroupName.Equals(groupName));
        }

        public ConnectionHistory? GetLastHistory(long gridId)
        {
            return _data.FirstOrDefault(history => history.GridId.Equals(gridId));
        }

        public int MaxPage(int amount)
        {
            return _data.Count / amount + 1;
        }

        public void Clear()
        {
            _data.Clear();
        }

        public void Save(MyIni ini)
        {
            var dataLines = string.Join(";", _data.Select(d => d.Serialize()));
            ini.Set(SaveKey, "data", dataLines);
        }

        public void Load(MyIni ini)
        {
            var split = ini.Get(SaveKey, "data")
                .ToString()
                .Split(';');

            _data.Clear();

            foreach (var line in split)
            {
                var history = ConnectionHistory.Deserialize(line);
                _data.Add(history);
            }
        }
    }
}