using System;
using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class RenameEntryCommand : ICommand
    {
        private readonly IMapEntryRepository _mapEntryRepository;
        private readonly IDetectionDataRepository _detectionDataRepository;

        public RenameEntryCommand(Program program)
        {
            _mapEntryRepository = program.Container.GetItem<IMapEntryRepository>();
            _detectionDataRepository = program.Container.GetItem<IDetectionDataRepository>();
        }

        public string[] Names => new[] { "rename", "ren" };

        public void Execute(MyCommandLine commandLine)
        {
            if (commandLine.ArgumentCount < 3)
                throw new Exception("Usage: rename <id|name|detected> <newName>");

            var identifier = commandLine.Argument(1);
            var newName = commandLine.Argument(2);

            IMapEntry entry = null;

            if (string.Equals(identifier, "detected", StringComparison.OrdinalIgnoreCase)
                || string.Equals(identifier, "d", StringComparison.OrdinalIgnoreCase))
            {
                if (!_detectionDataRepository.DetectedEntityInfo.HasValue)
                    throw new Exception("No entity is currently detected.");

                entry = _mapEntryRepository.GetOneById<IMapEntry>(
                    _detectionDataRepository.DetectedEntityInfo.Value.EntityId
                );

                if (entry == null)
                    throw new Exception("Detected entity is not yet saved in the database.");
            }
            else
            {
                long entityId;
                if (long.TryParse(identifier, out entityId))
                    entry = _mapEntryRepository.GetOneById<IMapEntry>(entityId);

                if (entry == null)
                    entry = _mapEntryRepository.GetOne<IMapEntry>(e =>
                        string.Equals(e.BaseName, identifier, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(e.CustomName, identifier, StringComparison.OrdinalIgnoreCase)
                    );

                if (entry == null)
                    throw new Exception($"No entry found for '{identifier}'");
            }

            entry.CustomName = newName;
        }

        public string GetUsage()
        {
            return "rename <id|baseName|customName|detected> <newName>  |  Alias: ren\n" +
                   "  detected (or d): rename the entity currently in the detection crosshair.";
        }
    }
}
