using System.Collections.Generic;
using System.Text;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    public class GeneralIhmModule : IIhmModule
    {
        private readonly Program _program;
        private readonly IMapEntryRepository _mapEntryRepository;

        public GeneralIhmModule(Program program)
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
            surface.Surface.WriteText(RenderText());
            yield return false;
        }

        private string RenderText()
        {
            var selfPosition = _program.Me.GetPosition();
            var sb = new StringBuilder();
            sb.AppendLine("|------- Space Map -------|");

            var asteroids = _mapEntryRepository.GetAll<Asteroid>();
            sb.AppendLine($"\n< Asteroids: {asteroids.Count} >");
            foreach (var asteroid in asteroids)
            {
                var name = string.IsNullOrEmpty(asteroid.CustomName)
                    ? asteroid.BaseName
                    : asteroid.CustomName;
                var distance = (long)Vector3D.Distance(asteroid.Position, selfPosition);
                var age = TimeUtils.FormatAge(asteroid.UpdateDate, shortFormat: true);
                var newFlag = TimeUtils.IsNew(asteroid.FirstDetectionDate) ? "[!] " : "    ";
                sb.AppendLine($"{newFlag}{name} ({distance}m, {age})");
            }

            return sb.ToString();
        }
    }
}
