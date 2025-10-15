using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class GroupDetectionSystem : IAsyncSystem
    {
        private const string StepMessage = "Detecting groups...\n";

        private readonly Program _program;
        private readonly DockGroupRepository _groupRepository;
        private readonly ScriptConfig _scriptConfig;

        public GroupDetectionSystem(Program program)
        {
            _program = program;
            _groupRepository = program.Container.GetItem<DockGroupRepository>();
            _scriptConfig = program.Container.GetItem<ScriptConfig>();
        }

        public IEnumerator<bool> Run()
        {
            // Get connectors
            var shipConnectors = new List<IMyShipConnector>();
            _program.GridTerminalSystem.GetBlocksOfType(
                shipConnectors,
                connector => _scriptConfig
                    .IdentificationRegex
                    .IsMatch(connector.CustomName)
            );
            _program.Echo(StepMessage + $"Matching {shipConnectors.Count} ship connectors");
            yield return true;

            // Matching names
            _program.Echo(StepMessage + "Resolving names...");
            var connectorsByName = new Dictionary<string, IMyShipConnector>();
            foreach (var connector in shipConnectors)
            {
                var match = _scriptConfig.IdentificationRegex.Match(connector.CustomName);
                var key = match.Groups[1].Value;

                if (connectorsByName.ContainsKey(key))
                {
                    _program.Echo($"[WARN] Duplicate connector group name '{key}', ignored. ");
                    continue;
                }

                connectorsByName[key] = connector;
            }

            yield return true;

            // Create groups for unknown connectors
            _program.Echo(StepMessage + "Detecting new groups...");
            foreach (var entry in connectorsByName)
            {
                var group = _groupRepository.GetGroup(entry.Key);
                if (group == null)
                {
                    group = new DockGroup
                    {
                        Name = entry.Key,
                        Connector = entry.Value,
                        IsConnected = entry.Value.IsConnected,
                    };
                    _groupRepository.RegisterGroup(group);

                    _program.Echo($"- New group detected: {entry.Key}");
                }
            }

            yield return true;

            // Removing unmatched groups
            _program.Echo(StepMessage + "Removing unused groups...");
            var names = connectorsByName.Keys.ToList();
            foreach (var entry in _groupRepository.GetAll())
            {
                if (!names.Contains(entry.Name))
                {
                    _groupRepository.RegisterGroup(entry);
                    _program.Echo($"- Group removed: {entry.Name}");
                }
            }
        }
    }
}