namespace IngameScript
{
    public interface ICommand
    {
        string[] Names { get; }

        void Execute(Program program, CommandInput input);

        string GetUsage();
    }
}