using VRageMath;

namespace IngameScript
{
    public class Asteroid : IMapEntry
    {
        public long Id { get; }

        public string BaseName { get; }

        public string CustomName { get; set; }

        public Vector3D Position { get; }

        public long UpdateDate { get; set; }

        public Asteroid(long id, string baseName, Vector3D position)
        {
            Id = id;
            BaseName = baseName;
            Position = position;
        }
    }
}