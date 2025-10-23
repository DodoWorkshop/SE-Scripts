using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame;

namespace IngameScript
{
    static class IMyGridTerminalSystemExtensions
    {
        public static List<T> GetBlockWithTag<T>(
            this IMyGridTerminalSystem self,
            string tag,
            Func<IMyTerminalBlock, bool> predicate = null,
            IMyCubeGrid grid = null
        )
            where T : class, IMyEntity
        {
            var extractor = new List<IMyTerminalBlock>();
            self.SearchBlocksOfName(tag, extractor, b =>
            {
                if (grid != null && !b.CubeGrid.Equals(grid))
                {
                    return false;
                }

                return predicate == null || predicate(b);
            });

            return extractor.OfType<T>()
                .ToList();
        }

        public static T GetUniqueBlockWithTag<T>(this IMyGridTerminalSystem self, string tag)
            where T : class, IMyEntity
        {
            var matches = GetBlockWithTag<T>(self, tag);
            if (matches.Count == 0)
            {
                return null;
            }

            CheckUnicity(matches, tag);

            return matches[0];
        }

        public static T GetUniqueRequiredBlockWithTag<T>(this IMyGridTerminalSystem self, string tag,
            IMyCubeGrid grid = null)
            where T : class, IMyEntity
        {
            var matches = GetBlockWithTag<T>(self, tag, null, grid);

            if (matches.Count == 0)
                throw new Exception($"No {typeof(T).Name.Remove(0, 3)} found with tag {tag}");


            CheckUnicity(matches, tag);

            return matches[0];
        }

        private static void CheckUnicity<T>(List<T> matches, string tag)
        {
            if (matches.Count > 1)
                throw new Exception(
                    $"More than one {typeof(T).Name.Remove(0, 3)} found with tag {tag}, this is not allowed");
        }

        /*

        public static T LoadUniqueBlockFromGroup<T>(
            this IMyGridTerminalSystem self,
            IMyBlockGroup group,
            Func<T, bool> predicate = null
        )
            where T : class, IMyEntity
        {
            var blocks = LoadBlocksFromGroup(self, group, predicate);

            if (blocks.Count > 1)
                throw new Exception(
                    $"More than one {typeof(T).Name.Remove(0, 3)} found in group {group.Name}, this is not allowed. Check if there if multiple groups with the same name"
                );

            return blocks.Count == 0 ? null : blocks[0];
        }

        public static T LoadUniqueRequiredBlockFromGroup<T>(
            this IMyGridTerminalSystem self,
            IMyBlockGroup group,
            Func<T, bool> predicate = null
        )
            where T : class, IMyEntity
        {
            var block = LoadUniqueBlockFromGroup(self, group, predicate);

            if (block == null)
                throw new Exception($"No {typeof(T).Name.Remove(0, 3)} found in group {group.Name}");

            return block;
        }

        public static List<T> LoadBlocksFromGroupName<T>(
            this IMyGridTerminalSystem self,
            string groupName,
            Func<T, bool> predicate = null
        )
            where T : class, IMyEntity
        {
            var group = self.GetBlockGroupWithName(groupName);
            if (group == null)
            {
                throw new Exception($"No group found with name {groupName}");
            }

            return LoadBlocksFromGroup(self, group, predicate);
        }

        public static List<T> LoadBlocksFromGroup<T>(
            this IMyGridTerminalSystem self,
            IMyBlockGroup group,
            Func<T, bool> predicate = null
        )
            where T : class, IMyEntity
        {
            var extractor = new List<T>();
            group.GetBlocksOfType(extractor, predicate);
            return extractor;
        }

        public static List<T> LoadBlocksFromTag<T>(this IMyGridTerminalSystem self, string tag)
            where T : class, IMyEntity
        {
            var extractor = new List<IMyTerminalBlock>();
            self.SearchBlocksOfName(tag, extractor);

            return extractor.OfType<T>().ToList();
        }

        public static T FindFirstOnGrid<T>(this IMyGridTerminalSystem self, IMyCubeGrid grid) where T : class, IMyTerminalBlock
        {
            var collector = new List<T>();
            self.GetBlocksOfType(collector);

            return collector.FirstOrDefault(b => b.CubeGrid.Equals(grid));
        }*/
    }
}