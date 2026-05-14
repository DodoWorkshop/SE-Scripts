using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public interface ICommand
    {
        string[] Names { get; }

        string Execute(MyCommandLine commandLine);

        string GetUsage();
    }
}