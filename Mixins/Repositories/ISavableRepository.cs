using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public interface ISavableRepository : IRepository
    {
        void Save(MyIni ini);

        void Load(MyIni ini);
    }
}