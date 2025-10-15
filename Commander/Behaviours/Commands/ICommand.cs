using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public interface ICommand
    {
        string[] Names { get; }
        
        void Execute(Program program, CommandInput input);

        string GetUsage();
    }
}