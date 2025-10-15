using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public struct PanelSurface
    {
        public readonly IMyTextSurface Surface;

        public readonly DisplayMode Mode;

        public PanelSurface(IMyTextSurface surface, DisplayMode mode)
        {
            Surface = surface;
            Mode = mode;
        }
    }
}