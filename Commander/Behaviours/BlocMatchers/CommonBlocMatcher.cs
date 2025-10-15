using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    public class CommonBlocMatcher : IBlocMatcher
    {
        public List<string> GetOptionsUsages()
        {
            return new List<string>
            {
                "-tag=\"{string}\" : filters blocs containing provided string in their name",
                "-name=\"{string}\" : filters blocs with exact name",
                "-data={string} : filters blocs containing provided string in their custom data [TODO]",
                "-grid={bool} : if true, only the current grid will be considered (default: false)",
                "-group=\"{string}\" : keep only blocs in the provided group"
            };
        }

        public List<IMyTerminalBlock> GetMatchingBlocs(Program program, IEnumerable<CommandOption> options)
            => GetMatchingBlocs<IMyTerminalBlock>(program, options);

        public List<T> GetMatchingBlocs<T>(Program program, IEnumerable<CommandOption> options)
            where T : class, IMyTerminalBlock
        {
            // Read options
            var filters = new List<Func<T, bool>>();
            var gridLimit = false;
            string groupName = null;
            foreach (var option in options)
            {
                switch (option.Name)
                {
                    case "tag":
                        program.Echo($"Tag filter set to {option.Value}");
                        filters.Add(bloc => bloc.CustomName.Contains(option.Value));
                        break;

                    case "name":
                        program.Echo($"Name filter set to {option.Value}");
                        filters.Add(bloc => bloc.CustomName.Equals(option.Value));
                        break;

                    case "data":
                        throw new Exception("\"data\" option not supported yet");

                    case "grid":
                        switch (option.Value)
                        {
                            case "true":
                                gridLimit = true;
                                break;
                            case "false":
                                break;
                            default:
                                throw new Exception(
                                    $"\"{option.Value}\" is not a valid value for grid. Allowed values are: true, false");
                        }

                        program.Echo(gridLimit
                            ? "Selection limited to current grid"
                            : "No grid limit set for selection"
                        );
                        break;

                    case "group":
                        if (groupName != null)
                        {
                            throw new Exception("Only one group can be provided");
                        }

                        groupName = option.Value;
                        program.Echo($"Selection limited to group \"{groupName}\"");
                        break;

                    default:
                        throw new Exception($"{option.Name} is not a valid option");
                }
            }

            // Extract blocs
            var grid = program.Me.CubeGrid;
            var targets = new List<T>();
            if (groupName != null)
            {
                var group = program.GridTerminalSystem.GetBlockGroupWithName(groupName);
                if (group == null)
                {
                    throw new Exception($"No group named \"{groupName}\" found");
                }

                group.GetBlocksOfType(targets,
                    bloc =>
                    {
                        if (gridLimit && !bloc.CubeGrid.Equals(grid)) return false;
                        return filters.All(filter => filter(bloc));
                    });
            }
            else
            {
                program.GridTerminalSystem.GetBlocksOfType(targets,
                    bloc =>
                    {
                        if (gridLimit && !bloc.CubeGrid.Equals(grid)) return false;
                        return filters.All(filter => filter(bloc));
                    });
            }

            if (targets.Count == 0)
            {
                throw new Exception("No targets are matching the provided filters");
            }

            program.Echo($"{targets.Count} targets matching the provided filters");

            return targets;
        }
    }
}