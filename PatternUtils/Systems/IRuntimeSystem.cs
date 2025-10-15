using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public interface IRuntimeSystem : ISystem
    {
        void Run(string argument, UpdateType updateSource);
    }
}