using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public interface ISystemManager
    {
        void SetGroupUpdateTypes(string group, UpdateType updateType);

        void RegisterSystem(string group, ISystem system);

        void UnregisterSystem(string group, ISystem system);

        void RunSystems(string group, UpdateType updateSource);

        void RegisterSystems(string group, UpdateType updateSource, IEnumerable<ISystem> systems);

        UpdateFrequency GetUpdateFrequency();
    }
}