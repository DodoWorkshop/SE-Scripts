using System;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class ShipSyncSystem : IDatabaseSystem
    {
        private readonly Program _program;
        private readonly IMapEntryRepository _mapEntryRepository;
        private readonly SyncStats _syncStats;
        private int _tickCount;

        public ShipSyncSystem(Program program)
        {
            _program = program;
            _mapEntryRepository = program.Container.GetItem<IMapEntryRepository>();
            _syncStats = program.Container.GetItem<SyncStats>();
        }

        public void Run(string argument, UpdateType updateSource)
        {
            _tickCount++;
            if (_tickCount < Program.SyncBroadcastInterval) return;
            _tickCount = 0;
            Broadcast();
        }

        private void Broadcast()
        {
            var gridName = _program.Me.CubeGrid.CustomName;
            var entries = _mapEntryRepository.GetAll<IMapEntry>();
            var payload = gridName + "\n" + _mapEntryRepository.SerializeAll();
            _program.IGC.SendBroadcastMessage(Program.SyncChannel, payload, TransmissionDistance.TransmissionDistanceMax);
            _syncStats.LastBroadcastDate = DateTime.Now.Ticks;
            _syncStats.LastBroadcastEntryCount = entries.Count;
        }
    }
}
