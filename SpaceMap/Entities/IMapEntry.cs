using VRageMath;

namespace IngameScript
{
    public interface IMapEntry
    {
        string TypeKey { get; }

        long Id { get; }

        string BaseName { get; }

        string CustomName { get; set; }

        Vector3D Position { get; }

        long UpdateDate { get; set; }

        long FirstDetectionDate { get; }
    }
}