using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class GroupTextPanelDetectionSystem : IAsyncSystem
    {
        private const string StepMessage = "Detecting TextPanels...\n";

        private readonly Program _program;
        private readonly DockGroupRepository _groupRepository;
        private readonly MyIni _ini;
        private readonly ScriptConfig _scriptConfig;
        private readonly GroupTextPanelService _groupTextPanelService;

        public GroupTextPanelDetectionSystem(Program program)
        {
            _program = program;
            _groupRepository = program.Container.GetItem<DockGroupRepository>();
            _ini = new MyIni();
            _scriptConfig = program.Container.GetItem<ScriptConfig>();
            _groupTextPanelService = program.Container.GetItem<GroupTextPanelService>();
        }

        public IEnumerator<bool> Run()
        {
            // Find panels
            _program.Echo(StepMessage + "Getting panels...");
            var textPanels = new List<IMyTextPanel>();
            _program.GridTerminalSystem.GetBlocksOfType(
                textPanels,
                textPanel => _scriptConfig.IdentificationRegex.IsMatch(textPanel.CustomName)
            );
            yield return true;

            // Match panels
            _program.Echo(StepMessage + "Grouping panels...");
            var groupedTextPanels = textPanels.GroupBy(textPanel =>
                {
                    var match = _scriptConfig.IdentificationRegex.Match(textPanel.CustomName);
                    return match.Groups[1].Value;
                }
            );
            yield return true;

            // Assign panels
            _program.Echo(StepMessage + "Assigning panels...");
            foreach (var entry in groupedTextPanels)
            {
                var hasChanged = false;
                var group = _groupRepository.GetGroup(entry.Key);
                if (group != null)
                {
                    var input = entry.ToHashSet();

                    var toAdd = input.Except(group.TextPanels);
                    var toRemove = group.TextPanels.Except(input);

                    foreach (var textPanel in toAdd)
                    {
                        textPanel.CustomData = GenerateCustomData(textPanel);
                        hasChanged = true;
                        group.TextPanels.Add(textPanel);
                        _program.Echo($"- {group.Name}: {textPanel.CustomName} assigned");
                    }

                    foreach (var textPanel in toRemove)
                    {
                        hasChanged = true;
                        group.TextPanels.Remove(textPanel);
                        _program.Echo($"- {group.Name}: {textPanel.CustomName} unassigned");
                    }
                }

                if (hasChanged)
                {
                    _groupTextPanelService.RefreshPanels(group);
                }
            }

            yield return false;
        }

        private string GenerateCustomData(IMyTextPanel panel)
        {
            if (!_ini.TryParse(panel.CustomData))
            {
                _ini.Clear();
            }

            foreach (var section in new[] { DataStructure.ConnectedSection, DataStructure.DisconnectedSection })
            {
                if (!_ini.ContainsKey(section, DataStructure.FontSizeField))
                    _ini.Set(section, DataStructure.FontSizeField, panel.FontSize);

                if (!_ini.ContainsKey(section, DataStructure.TextAlignField))
                {
                    _ini.Set(section, DataStructure.TextAlignField, panel.Alignment.ToString());
                    _ini.SetComment(section, DataStructure.TextAlignField,
                        "Possible values are: " + string.Join(", ", Enum.GetNames(typeof(TextAlignment)))
                    );
                }

                if (!_ini.ContainsKey(section, DataStructure.TextPaddingField))
                    _ini.Set(section, DataStructure.TextPaddingField, panel.TextPadding);
            }

            return _ini.ToString();
        }
    }
}