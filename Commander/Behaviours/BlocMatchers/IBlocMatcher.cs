using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public interface IBlocMatcher
    {
        List<string> GetOptionsUsages();

        List<IMyTerminalBlock> GetMatchingBlocs(Program program, IEnumerable<CommandOption> options);

        List<T> GetMatchingBlocs<T>(Program program, IEnumerable<CommandOption> options)
            where T : class, IMyTerminalBlock;
    }
}