using System.Collections.Generic;

namespace IngameScript
{
    public interface IIhmModule
    {
        void InitSurface(Panel panel, PanelSurface surface);

        IEnumerator<bool> RenderTo(Panel panel, PanelSurface surface);
    }
}