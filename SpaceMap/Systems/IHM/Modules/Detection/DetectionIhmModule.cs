using System.Collections.Generic;
using System.Text;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class DetectionIhmModule : IIhmModule
    {
        private readonly Program _program;
        private readonly IDetectionDataRepository _detectionDataRepository;
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly IMapEntryRepository _mapEntryRepository;

        public DetectionIhmModule(Program program)
        {
            _program = program;
            _detectionDataRepository = program.Container.GetItem<IDetectionDataRepository>();
            _userSettingsRepository = program.Container.GetItem<IUserSettingsRepository>();
            _mapEntryRepository = program.Container.GetItem<IMapEntryRepository>();
        }

        public void InitSurface(Panel panel, PanelSurface surface)
        {
            surface.Surface.ContentType = ContentType.TEXT_AND_IMAGE;
        }

        public IEnumerator<bool> RenderTo(Panel panel, PanelSurface surface)
        {
            var sb = new StringBuilder();
            sb.AppendLine("< DETECTION >");

            sb.AppendLine("\n[ Sensor ]");
            sb.AppendLine($"Ray charge: {_detectionDataRepository.RaycastCharge:P1}");
            sb.AppendLine($"Max range:  {_userSettingsRepository.DetectionDistance}m");

            sb.AppendLine("\n[ Detected Entity ]");
            if (_detectionDataRepository.DetectedEntityInfo.HasValue)
            {
                var result = _detectionDataRepository.DetectedEntityInfo.Value;

                if (result.Type != MyDetectedEntityType.Asteroid)
                {
                    sb.AppendLine("Status: Not handled");
                    sb.AppendLine($"Type: {result.Type}");
                }
                else
                {
                    var entry = _mapEntryRepository.GetOneById<IMapEntry>(result.EntityId);
                    if (entry == null)
                    {
                        sb.AppendLine("Status: Saving...");
                    }
                    else
                    {
                        var isNew = TimeUtils.IsNew(entry.UpdateDate);
                        sb.AppendLine($"Status: {(isNew ? "[NEW] " : "")}Known");
                        sb.AppendLine($"Type:   {entry.GetType().Name}");
                        sb.AppendLine($"Name:   {entry.BaseName}");
                        if (!string.IsNullOrEmpty(entry.CustomName))
                            sb.AppendLine($"Alias:  {entry.CustomName}");
                        sb.AppendLine($"Seen:   {TimeUtils.FormatAge(entry.UpdateDate)}");
                    }
                }

                var distance = Vector3D.Distance(result.Position, _program.Me.GetPosition());
                sb.AppendLine($"Dist:   {distance:0}m");
                sb.AppendLine($"ID:     {result.EntityId}");
            }
            else
            {
                sb.AppendLine("Nothing detected");
            }

            surface.Surface.WriteText(sb.ToString());
            yield return false;
        }
    }
}
