using VRageMath;

namespace IngameScript
{
    public class Asteroid : IMapEntry
    {
        public string TypeKey => "Asteroid";

        public long Id { get; }

        public string BaseName { get; }

        public string CustomName { get; set; }

        public Vector3D Position { get; }

        public long UpdateDate { get; set; }

        public long FirstDetectionDate { get; }

        public Asteroid(long id, string baseName, Vector3D position, long firstDetectionDate = 0)
        {
            Id = id;
            BaseName = baseName;
            Position = position;
            FirstDetectionDate = firstDetectionDate;
        }
    }
}