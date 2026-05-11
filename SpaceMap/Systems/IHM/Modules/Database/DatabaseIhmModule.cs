using System.Collections.Generic;
using System.Text;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class DatabaseIhmModule : IIhmModule
    {
        private readonly Program _program;
        private readonly IMapEntryRepository _mapEntryRepository;

        public DatabaseIhmModule(Program program)
        {
            _program = program;
            _mapEntryRepository = program.Container.GetItem<IMapEntryRepository>();
        }

        public void InitSurface(Panel panel, PanelSurface surface)
        {
            surface.Surface.ContentType = ContentType.TEXT_AND_IMAGE;
        }

        public IEnumerator<bool> RenderTo(Panel panel, PanelSurface surface)
        {
            var sb = new StringBuilder();
            sb.AppendLine("< DATABASE >");

            var selfPosition = _program.Me.GetPosition();
            var asteroids = _mapEntryRepository.GetAll<Asteroid>();
            sb.AppendLine($"Entries: {asteroids.Count}\n");

            sb.AppendLine("[ Asteroids ]");
            foreach (var entry in asteroids)
            {
                var isNew = TimeUtils.IsNew(entry.UpdateDate);
                var displayName = string.IsNullOrEmpty(entry.CustomName)
                    ? entry.BaseName
                    : $"{entry.BaseName} \"{entry.CustomName}\"";
                var age = TimeUtils.FormatAge(entry.UpdateDate);
                var distance = (long)Vector3D.Distance(entry.Position, selfPosition);

                sb.AppendLine($"{(isNew ? "[!] " : "    ")}{displayName}");
                sb.AppendLine($"      ID:{entry.Id} | {distance}m | {age}");
            }

            surface.Surface.WriteText(sb.ToString());
            yield return false;
        }
    }
}
