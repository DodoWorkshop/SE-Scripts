using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class SystemManager : ISystemManager
    {
        private readonly MyGridProgram _program;

        private readonly Dictionary<string, IEnumerator<bool>> _asyncSystemCoroutines
            = new Dictionary<string, IEnumerator<bool>>();

        private readonly Dictionary<string, List<ISystem>> _systems = new Dictionary<string, List<ISystem>>();
        private readonly Dictionary<string, UpdateFrequency> _updateFrequencies = new Dictionary<string, UpdateFrequency>();

        public SystemManager(MyGridProgram program)
        {
            _program = program;
        }

        public void SetGroupUpdateFrequency(string groupName, UpdateFrequency updateFrequency)
        {
            _updateFrequencies[groupName] = updateFrequency;
        }

        public void RegisterSystem(string group, ISystem system)
        {
            List<ISystem> content;
            if (_systems.TryGetValue(group, out content))
            {
                content.Add(system);
            }
            else
            {
                _systems.Add(group, new List<ISystem> { system });
            }
        }

        public void UnregisterSystem(string group, ISystem system)
        {
            List<ISystem> content;
            if (!_systems.TryGetValue(group, out content))
            {
                throw new Exception($"No system for type {system.GetType()} registered in group {group}");
            }

            content.Remove(system);
        }

        public void RunSystems(string argument, UpdateType updateType)
        {
            var updateFrequency = UpdateFrequency.None;
            switch (updateType)
            {
                case UpdateType.Update1:
                    updateFrequency = UpdateFrequency.Update1;
                    break;
                case UpdateType.Update10:
                    updateFrequency = UpdateFrequency.Update10;
                    break;
                case UpdateType.Update100:
                    updateFrequency = UpdateFrequency.Update100;
                    break;
            }
            
            foreach (var groupKey in _systems.Keys)
            {
                IEnumerator<bool> coroutine;
                
                // Check if frequency has been set
                UpdateFrequency groupUpdateFrequency;
                if (_updateFrequencies.TryGetValue(groupKey, out groupUpdateFrequency))
                {
                    // Skip if no match
                    if (!groupUpdateFrequency.HasFlag(updateFrequency)) continue;
                }
                
                // Attempt to get coroutine
                if (!_asyncSystemCoroutines.TryGetValue(groupKey, out coroutine))
                {
                    coroutine = RunLoop(argument, updateType, groupKey);
                    _asyncSystemCoroutines.Add(groupKey, coroutine);
                }

                // Check next
                if (coroutine.MoveNext() && coroutine.Current) continue;
                coroutine.Dispose();

                _asyncSystemCoroutines.Remove(groupKey);
            }
        }

        public void RegisterSystems(string group, UpdateFrequency updateFrequency, IEnumerable<ISystem> systems)
        {
            SetGroupUpdateFrequency(group, updateFrequency);
            foreach (var system in systems)
            {
                RegisterSystem(group, system);
            }
        }

        public UpdateFrequency GetUpdateFrequency()
        {
            return _updateFrequencies.Values
                .Distinct()
                .Aggregate(UpdateFrequency.None, (current, updateFrequency) => current | updateFrequency);
        }

        private IEnumerator<bool> RunLoop(string argument, UpdateType updateSource, string group)
        {
            foreach (var system in _systems[group])
            {
                if (system is IRuntimeSystem)
                {
                    var runtimeSystem = (IRuntimeSystem)system;
                    try
                    {
                        runtimeSystem.Run(argument, updateSource);
                    }
                    catch (Exception e)
                    {
                        _program.Echo($"\n{e.Message}");
                    }
                }
                else if (system is IAsyncSystem)
                {
                    var asyncSystem = (IAsyncSystem)system;
                    var coroutine = asyncSystem.Run();
                    var hasNext = true;
                    while (hasNext)
                    {
                        try
                        {
                            hasNext = coroutine.MoveNext();
                        }
                        catch (Exception e)
                        {
                            _program.Echo($"\n{e.Message}");
                            break;
                        }

                        yield return true;
                    }

                    coroutine.Dispose();
                }
            }

            yield return false;
        }
    }
}