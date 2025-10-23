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
        private readonly Dictionary<string, UpdateType> _updateTypes = new Dictionary<string, UpdateType>();

        public SystemManager(MyGridProgram program)
        {
            _program = program;
        }

        public void SetGroupUpdateTypes(string groupName, UpdateType updateType)
        {
            _updateTypes[groupName] = updateType;
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

        public void RunSystems(string argument, UpdateType updateSource)
        {
            foreach (var groupKey in _systems.Keys)
            {
                UpdateType runtime;
                if (!_updateTypes.TryGetValue(groupKey, out runtime))
                {
                    throw new Exception($"No update type declared for group {groupKey}");
                }

                if (!runtime.HasFlag(updateSource)) continue;

                IEnumerator<bool> coroutine;
                if (!_asyncSystemCoroutines.TryGetValue(groupKey, out coroutine))
                {
                    coroutine = RunLoop(argument, updateSource, groupKey);
                    _asyncSystemCoroutines.Add(groupKey, coroutine);
                }

                if (coroutine.MoveNext()) continue;
                coroutine.Dispose();

                _asyncSystemCoroutines.Remove(groupKey);
            }
        }

        public void RegisterSystems(string group, UpdateType updateSource, IEnumerable<ISystem> systems)
        {
            SetGroupUpdateTypes(group, updateSource);
            foreach (var system in systems)
            {
                RegisterSystem(group, system);
            }
        }

        public UpdateFrequency GetUpdateFrequency()
        {
            var result = UpdateFrequency.None;
            foreach (var updateType in _updateTypes.Values.Distinct())
            {
                switch (updateType)
                {
                    case UpdateType.Update1:
                        result |= UpdateFrequency.Update1;
                        break;
                    case UpdateType.Update10:
                        result |= UpdateFrequency.Update10;
                        break;
                    case UpdateType.Update100:
                        result |= UpdateFrequency.Update100;
                        break;
                }
            }

            return result;
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

                    yield return true;
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