using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class ServerDatabaseSystem : IDatabaseSystem
    {
        private readonly IMapEntryRepository _mapEntryRepository;
        private readonly SyncStats _syncStats;
        private readonly IMyBroadcastListener _listener;

        public ServerDatabaseSystem(Program program)
        {
            _mapEntryRepository = program.Container.GetItem<IMapEntryRepository>();
            _syncStats = program.Container.GetItem<SyncStats>();
            _listener = program.IGC.RegisterBroadcastListener(Program.SyncChannel);
        }

        public void Run(string argument, UpdateType updateSource)
        {
            while (_listener.HasPendingMessage)
            {
                var msg = _listener.AcceptMessage();
                var data = msg.Data as string;
                if (data == null) continue;
                ProcessMessage(msg.Source, data);
            }
        }

        private void ProcessMessage(long sourceId, string data)
        {
            var sep = data.IndexOf('\n');
            if (sep < 0) return;

            var sourceName = data.Substring(0, sep);
            var entries = data.Substring(sep + 1);

            _mapEntryRepository.MergeFrom(entries);

            var count = 0;
            foreach (var line in entries.Split('\n'))
                if (!string.IsNullOrWhiteSpace(line)) count++;

            _syncStats.RegisterSource(sourceId, sourceName, count);
        }
    }
}
