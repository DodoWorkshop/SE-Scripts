using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public interface IIhmModule
    {
        void InitSurface(Panel panel, PanelSurface surface);

        IEnumerator<bool> RenderTo(Panel panel, PanelSurface surface);
    }
}