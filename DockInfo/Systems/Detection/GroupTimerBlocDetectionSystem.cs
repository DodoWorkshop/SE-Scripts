using System;
using System.Collections.Generic;
using System.Linq;
using SpaceEngineers.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class GroupTimerBlocDetectionSystem : IAsyncSystem
    {
        private const string StepMessage = "Detecting Timer Blocks...\n";

        private readonly Program _program;
        private readonly DockGroupRepository _groupRepository;
        private readonly MyIni _ini;
        private readonly ScriptConfig _scriptConfig;

        public GroupTimerBlocDetectionSystem(Program program)
        {
            _program = program;
            _groupRepository = program.Container.GetItem<DockGroupRepository>();
            _ini = new MyIni();
            _scriptConfig = program.Container.GetItem<ScriptConfig>();
        }

        public IEnumerator<bool> Run()
        {
            // Find timer blocks
            _program.Echo(StepMessage + "Getting timer blocks...");
            var timerBlocks = new List<IMyTimerBlock>();
            _program.GridTerminalSystem.GetBlocksOfType(
                timerBlocks,
                timerBlock => _scriptConfig.IdentificationRegex.IsMatch(timerBlock.CustomName)
            );
            yield return true;

            // Match groups
            _program.Echo(StepMessage + "Grouping blocs...");
            var groupedBlocs = timerBlocks.GroupBy(timerBlock =>
                {
                    var match = _scriptConfig.IdentificationRegex.Match(timerBlock.CustomName);
                    return match.Groups[1].Value;
                }
            );
            yield return true;

            // Assign panels
            _program.Echo(StepMessage + "Assigning blocs...");
            foreach (var entry in groupedBlocs)
            {
                var group = _groupRepository.GetGroup(entry.Key);
                if (group != null)
                {
                    var input = entry.ToHashSet();

                    var toAdd = input.Except(group.TimerBlocks);
                    var toRemove = group.TimerBlocks.Except(input);

                    foreach (var timerBloc in toAdd)
                    {
                        timerBloc.CustomData = GenerateCustomData(timerBloc);
                        group.TimerBlocks.Add(timerBloc);
                        _program.Echo($"- {group.Name}: {timerBloc.CustomName} assigned");
                    }

                    foreach (var timerBlock in toRemove)
                    {
                        group.TimerBlocks.Remove(timerBlock);
                        _program.Echo($"- {group.Name}: {timerBlock.CustomName} unassigned");
                    }
                }
            }

            yield return false;
        }

        private string GenerateCustomData(IMyTimerBlock bloc)
        {
            if (!_ini.TryParse(bloc.CustomData))
            {
                _ini.Clear();
            }

            foreach (var section in new[] { DataStructure.ConnectedSection, DataStructure.DisconnectedSection })
            {
                if (!_ini.ContainsSection(section))
                {
                    _ini.AddSection(section);
                }
                
                if (!_ini.ContainsKey(section, DataStructure.TimerBlocActionField))
                {
                    _ini.Set(section, DataStructure.TimerBlocActionField, nameof(TimerBlocAction.None));
                    _ini.SetComment(section, DataStructure.TimerBlocActionField,
                        "Possible values are: " + string.Join(", ", Enum.GetNames(typeof(TimerBlocAction)))
                    );
                }
            }

            return _ini.ToString();
        }
    }
}