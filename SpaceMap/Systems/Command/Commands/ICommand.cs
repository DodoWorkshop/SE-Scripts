using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public interface ICommand
    {
        string[] Names { get; }
        
        void Execute(MyCommandLine commandLine);

        string GetUsage();
    }
}