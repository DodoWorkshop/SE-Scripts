using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class UserSettingsRepository : IUserSettingsRepository
    {
        private const string DetectionDistanceKey = "detectionDistance";
        private const string MapScaleKey = "mapScale";

        public uint DetectionDistance { get; set; } = 10000;

        public uint MapScale { get; set; } = 5000;

        public void Save(MyIni ini)
        {
            var sectionKey = GetType().Name;
            ini.AddSection(sectionKey);
            ini.Set(sectionKey, DetectionDistanceKey, DetectionDistance);
            ini.Set(sectionKey, MapScaleKey, MapScale);
        }

        public void Load(MyIni ini)
        {
            var sectionKey = GetType().Name;
            DetectionDistance = ini.Get(sectionKey, DetectionDistanceKey).ToUInt32();
            MapScale = ini.Get(sectionKey, MapScaleKey).ToUInt32();
        }
    }
}