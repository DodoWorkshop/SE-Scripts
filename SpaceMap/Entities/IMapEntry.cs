using VRageMath;

namespace IngameScript
{
    public interface IMapEntry
    {
        long Id { get; }
        
        string BaseName { get; }
        
        string CustomName { get; set; }
        
        Vector3D Position { get; }
        
        long UpdateDate { get; set; }
    }
}