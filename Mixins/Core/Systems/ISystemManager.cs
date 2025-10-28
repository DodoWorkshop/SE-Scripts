using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public interface ISystemManager
    {
        void SetGroupUpdateFrequency(string group, UpdateFrequency updateFrequency);

        void RegisterSystem(string group, ISystem system);

        void UnregisterSystem(string group, ISystem system);

        void RunSystems(string group, UpdateType updateSource);

        void RegisterSystems(string group, UpdateFrequency updateFrequency, IEnumerable<ISystem> systems);

        UpdateFrequency GetUpdateFrequency();
    }
}