using System;
using System.Collections.Generic;

namespace IngameScript
{
    public class SyncSource
    {
        public long EntityId;
        public string Name;
        public long LastSyncDate;
        public int EntryCount;
    }

    public class SyncStats
    {
        public ScriptMode Mode { get; }

        public long LastBroadcastDate { get; set; }
        public int LastBroadcastEntryCount { get; set; }

        private readonly List<SyncSource> _sources = new List<SyncSource>();
        public List<SyncSource> Sources => _sources;

        public SyncStats(ScriptMode mode)
        {
            Mode = mode;
        }

        public void RegisterSource(long entityId, string name, int entryCount)
        {
            for (var i = 0; i < _sources.Count; i++)
            {
                if (_sources[i].EntityId != entityId) continue;
                _sources[i].Name = name;
                _sources[i].LastSyncDate = DateTime.Now.Ticks;
                _sources[i].EntryCount = entryCount;
                return;
            }
            _sources.Add(new SyncSource
            {
                EntityId = entityId,
                Name = name,
                LastSyncDate = DateTime.Now.Ticks,
                EntryCount = entryCount
            });
        }
    }
}
