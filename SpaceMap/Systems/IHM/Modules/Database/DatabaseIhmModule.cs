using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    public class DatabaseIhmModule : IIhmModule
    {
        public void InitSurface(Panel panel, PanelSurface surface)
        {
            surface.Surface.ContentType = ContentType.TEXT_AND_IMAGE;
        }

        public IEnumerator<bool> RenderTo(Panel panel, PanelSurface surface)
        {
            surface.Surface.WriteText("WIP Database module");
            yield return false;
        }
    }
}