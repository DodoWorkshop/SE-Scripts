using VRage.Game.ModAPI.Ingame.Utilities;

namespace IngameScript
{
    public class UserSettingsRepository : IUserSettingsRepository
    {
        private const string RepositorySectionKey = "UserSettingsRepository";
        private const string DetectionDistanceKey = "detectionDistance";
        private const string MapScaleKey = "mapScale";

        public uint DetectionDistance { get; set; } = Program.DefaultDetectionDistance;

        public uint MapScale { get; set; } = Program.DefaultMapScale;

        public void Save(MyIni ini)
        {
            ini.AddSection(RepositorySectionKey);
            ini.Set(RepositorySectionKey, DetectionDistanceKey, DetectionDistance);
            ini.Set(RepositorySectionKey, MapScaleKey, MapScale);
        }

        public void Load(MyIni ini)
        {
            if (ini.ContainsKey(RepositorySectionKey, DetectionDistanceKey))
                DetectionDistance = ini.Get(RepositorySectionKey, DetectionDistanceKey).ToUInt32();
            if (ini.ContainsKey(RepositorySectionKey, MapScaleKey))
                MapScale = ini.Get(RepositorySectionKey, MapScaleKey).ToUInt32();
        }
    }
}