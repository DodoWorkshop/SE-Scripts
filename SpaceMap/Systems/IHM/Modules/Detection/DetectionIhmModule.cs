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
        private readonly IDetectionDataStore _detectionDataStore;
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly IMapEntryRepository _mapEntryRepository;

        public DetectionIhmModule(Program program)
        {
            _program = program;
            _detectionDataStore = program.StoreManager.GetStore<IDetectionDataStore>();
            _userSettingsRepository = program.RepositoryManager.GetRepository<IUserSettingsRepository>();
            _mapEntryRepository = program.RepositoryManager.GetRepository<IMapEntryRepository>();
        }

        public void InitSurface(Panel panel, PanelSurface surface)
        {
            surface.Surface.ContentType = ContentType.TEXT_AND_IMAGE;
        }

        public IEnumerator<bool> RenderTo(Panel panel, PanelSurface surface)
        {
            var sb = new StringBuilder();
            sb.AppendLine("< DETECTION >");
            
            sb.AppendLine("\n[ Info ]");
            sb.AppendLine($"Detection ray charge: {_detectionDataStore.RaycastCharge:P1}");
            sb.AppendLine($"Detection distance: {_userSettingsRepository.DetectionDistance}m");
            
            sb.AppendLine("\n[ Detected Entity ]");
            if (_detectionDataStore.DetectedEntityInfo.HasValue)
            {
                var result = _detectionDataStore.DetectedEntityInfo.Value;

                // TODO: manage handled types more clearly
                if (result.Type != MyDetectedEntityType.Asteroid)
                {
                    sb.AppendLine("Database status: Not handled");
                    sb.AppendLine($"Type: {result.Type}");
                }
                else
                {
                    var entry =  _mapEntryRepository.GetOneById<IMapEntry>(result.EntityId);
                    if (entry == null)
                    {
                        sb.AppendLine("Database status: Saving new entry...");
                    }
                    else
                    {
                        sb.AppendLine("Database status: Saved");
                        sb.AppendLine($"Type: {entry.GetType().Name}");
                        sb.AppendLine($"Base name: {entry.BaseName}");
                        sb.AppendLine($"Custom name: {entry.CustomName}");
                    }
                }
                
                var distance = Vector3D.Distance(result.Position, _program.Me.GetPosition());
                sb.AppendLine($"Distance: {distance:0}m");
                sb.AppendLine($"Id: {result.EntityId}");
            }
            else
            {
                sb.AppendLine("Noting has been detected");
            }
            
            surface.Surface.WriteText(sb.ToString());
            
            yield return false;
        }
    }
}