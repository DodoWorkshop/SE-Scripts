using System.Collections.Generic;
using System.Text;
using Sandbox.ModAPI.Ingame;
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
            _mapEntryRepository = program.RepositoryManager.GetRepository<IMapEntryRepository>();
        }

        public void InitSurface(Panel panel, PanelSurface surface)
        {
            surface.Surface.ContentType = ContentType.TEXT_AND_IMAGE;
        }

        public IEnumerator<bool> RenderTo(Panel panel, PanelSurface surface)
        {
            var text = RenderText();
            surface.Surface.WriteText(text);
            //_program.Echo(text);

            yield return false;
        }

        private string RenderText()
        {
            var selfPosition = _program.Me.GetPosition();

            var sb = new StringBuilder();
            sb.AppendLine("|---------- Space Map ----------|");

            sb.AppendLine("\n< Database: Asteroids >");
            foreach (var asteroid in _mapEntryRepository.GetAll<Asteroid>())
            {
                var nameToUse = string.IsNullOrEmpty(asteroid.CustomName)
                    ? asteroid.BaseName
                    : asteroid.CustomName;
                var distance = Vector3D.Distance(asteroid.Position, selfPosition);
                sb.AppendLine($"- {nameToUse} {distance:0}m)");
            }

            return sb.ToString();
        }
    }
}