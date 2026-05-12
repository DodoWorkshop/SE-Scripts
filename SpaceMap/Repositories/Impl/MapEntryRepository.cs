using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRageMath;

namespace IngameScript
{
    public class MapEntryRepository : IMapEntryRepository
    {
        private readonly Dictionary<long, IMapEntry> _mapEntries = new Dictionary<long, IMapEntry>();

        private const string MapEntriesSaveKey = "mapEntries";
        private const string MapEntriesSectionKey = "MapEntryRepository";
        private const char Sep = '|';

        public void Save(IMapEntry entry)
        {
            if (entry.Id > 0)
            {
                _mapEntries[entry.Id] = entry;
            }
            else
            {
                _mapEntries.Add(entry.Id, entry);
            }
        }

        public List<T> GetAll<T>(Func<T, bool> predicate = null) where T : IMapEntry
        {
            var query = _mapEntries.Values
                .OfType<T>();

            return predicate != null
                ? query.Where(predicate).ToList()
                : query.ToList();
        }

        public T GetOne<T>(Func<T, bool> predicate) where T : IMapEntry
        {
            return _mapEntries.Values
                .OfType<T>()
                .FirstOrDefault(predicate);
        }

        public T GetOneById<T>(long id) where T : IMapEntry
        {
            IMapEntry entry;
            if (_mapEntries.TryGetValue(id, out entry))
            {
                if (entry is T)
                {
                    return (T)entry;
                }

                throw new Exception(
                    $"An entry has been found with id {id}, but its type ({entry.GetType()}) is not of the requested type {typeof(T)})");
            }

            return default(T);
        }

        public void Clear()
        {
            _mapEntries.Clear();
        }

        public List<T> GetAllInArea<T>(Vector3D center, long radius) where T : IMapEntry
        {
            var radiusSqr = radius * radius;

            return _mapEntries.Values
                .OfType<T>()
                .Where(entry => (entry.Position - center).LengthSquared() <= radiusSqr)
                .ToList();
        }

        public void Save(MyIni ini)
        {
            // TODO: handle potential other types
            var lines = _mapEntries.Values
                .Select(entry =>
                {
                    var inv = System.Globalization.CultureInfo.InvariantCulture;
                    var data = new[]
                    {
                        entry.GetType().Name,
                        entry.Id.ToString(inv),
                        entry.BaseName,
                        entry.CustomName ?? "",
                        entry.Position.X.ToString(inv),
                        entry.Position.Y.ToString(inv),
                        entry.Position.Z.ToString(inv),
                        entry.UpdateDate.ToString(inv),
                        entry.FirstDetectionDate.ToString(inv)
                    };

                    return string.Join(Sep.ToString(), data);
                })
                .ToArray();

            var mapData = string.Join("\n", lines);

            ini.AddSection(MapEntriesSectionKey);
            ini.Set(MapEntriesSectionKey, MapEntriesSaveKey, mapData);
        }

        public void Load(MyIni ini)
        {
            if (!ini.ContainsKey(MapEntriesSectionKey, MapEntriesSaveKey)) return;

            var mapData = ini.Get(MapEntriesSectionKey, MapEntriesSaveKey).ToString();

            if (string.IsNullOrWhiteSpace(mapData)) return;

            var lines = mapData.Split('\n');
            _mapEntries.Clear();

            foreach (var line in lines)
            {
                var entry = LineToEntry(line);
                _mapEntries.Add(entry.Id, entry);
            }
        }

        private IMapEntry LineToEntry(string line)
        {
            // Backward compat: old saves used ',' as separator
            var split = line.IndexOf(Sep) >= 0 ? line.Split(Sep) : line.Split(',');
            var type = split[0];
            var inv = System.Globalization.CultureInfo.InvariantCulture;

            switch (type)
            {
                case "Asteroid":
                    return new Asteroid(
                        long.Parse(split[1], inv),
                        split[2],
                        new Vector3D(
                            double.Parse(split[4], inv),
                            double.Parse(split[5], inv),
                            double.Parse(split[6], inv)
                        ),
                        split.Length > 8 ? long.Parse(split[8], inv) : 0L
                    )
                    {
                        CustomName = split[3],
                        UpdateDate = long.Parse(split[7], inv)
                    };
                default:
                    throw new Exception($"Failed to read line entry: Unknown type '{type}'");
            }
        }
    }
}