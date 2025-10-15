using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class ShipBindings
    {
        public IMyCameraBlock Camera;

        public bool IsBound => Camera != null;

        public bool TryBind(Program program)
        {
            Camera = program.GridTerminalSystem.GetUniqueRequiredBlockWithTag<IMyCameraBlock>(
                Settings.NameTag,
                program.Me.CubeGrid
            );

            return IsBound;
        }
    }
}