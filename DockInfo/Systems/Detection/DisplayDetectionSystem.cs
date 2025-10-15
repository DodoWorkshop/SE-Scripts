using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class DisplayDetectionSystem : IAsyncSystem
    {
        private const string StepMessage = "Detecting Displays...\n";

        private readonly Program _program;
        private readonly ScriptConfig _config;
        private readonly DisplayRepository _displayRepository;

        public DisplayDetectionSystem(Program program)
        {
            _program = program;
            _config = _program.Container.GetItem<ScriptConfig>();
            _displayRepository = _program.Container.GetItem<DisplayRepository>();
        }

        public IEnumerator<bool> Run()
        {
            // Detecting history LCD
            _program.Echo(StepMessage + "Detecting history panels");
            var extractor = new List<IMyTerminalBlock>();
            _program.GridTerminalSystem.SearchBlocksOfName(ScriptConfig.HistoryPanelTag, extractor);

            var panels = extractor.OfType<IMyTextPanel>()
                .ToList();

            foreach (var panel in panels)
            {
                if (!_displayRepository.History.Any(existing => existing.Panel.Equals(panel)))
                {
                    _displayRepository.History.Add(new HistoryDisplay(panel));
                    _program.Echo($"Added: {panel.CustomName}");
                }
            }

            yield return true;

            // Clean history LCD
            _program.Echo(StepMessage + "Cleaning history panels");
            _displayRepository.History = _displayRepository.History
                .Where(existing => panels.Any(newPanel => newPanel.Equals(existing.Panel)))
                .ToList();
            yield return true;
        }
    }
}