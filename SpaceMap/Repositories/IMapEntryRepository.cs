using System;
using System.Collections.Generic;
using VRageMath;

namespace IngameScript
{
    public interface IMapEntryRepository : ISavableRepository
    {
        void Save(IMapEntry entry);

        List<T> GetAll<T>(Func<T, bool> predicate = null) where T : IMapEntry;

        T GetOne<T>(Func<T, bool> predicate) where T : IMapEntry;

        T GetOneById<T>(long id) where T : IMapEntry;

        void Clear();

        List<T> GetAllInArea<T>(Vector3D center, long radius)  where T : IMapEntry;
    }
}